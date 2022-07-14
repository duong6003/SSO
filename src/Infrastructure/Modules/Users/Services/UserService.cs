using AutoMapper;
using Core.Common.Interfaces;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using Infrastructure.Modules.Users.Utilities;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Core.Utilities;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Modules.Users.Services;

public interface IUserService : IScopedService
{
    Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId);
    Task<(User? User, string? ErrorMessage)> GetProfileAsync(Guid userId);

    Task<PaginationResponse<User>?> GetAllAsync(PaginationRequest request);

    Task<User> CreateAsync(UserSignUpRequest request);

    Task<(User User, string? ErrorMessage)> UpdateAsync(Guid userId, UpdateUserRequest request);
    Task<string?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<string?> DeleteAsync(Guid userId);

    Task<(List<Permission>? Permissions, string? ErrorMessage)> GetPermissionByUserAsync(Guid userId);
    Task AddPermissionByUserAsync(Guid userId, List<CreateUserPermissionRequest> request);
    Task UpdatePermissionByUserAsync(Guid userId, List<UpdateUserPermissionRequest> request);
    Task<(string? AccesToken, string? ErrorMessage)> AuthenticateAsync(UserSignInRequest request);
    Task<(User? User, string? ErrorMessage)> ResetPasswordAsync(Guid userId, ResetPasswordRequest request);
}

public class UserService : IUserService
{
    private readonly IRepositoryWrapper RepositoryWrapper;
    private readonly IMapper Mapper;
    private readonly IConfiguration Configuration;
    private readonly IFileService FileService;


    public UserService(IConfiguration configuration, IRepositoryWrapper repositoryWrapper, IMapper mapper, IFileService fileService)
    {
        RepositoryWrapper = repositoryWrapper;
        Configuration = configuration;
        Mapper = mapper;
        FileService = fileService;
    }

    private async Task<User?> GetByIdAsync(Guid userId)
    {
        User? user = await RepositoryWrapper.Users.GetByIdAsync(userId);
        return user;
    }

    public async Task<User?> GetByUserOrEmailAsync(string signUpName)
    {
        return await RepositoryWrapper.Users.Find(x => x.EmailAddress == signUpName || x.UserName == signUpName).FirstOrDefaultAsync();
    }

    public async Task<(User? User, string? ErrorMessage)> GetDetailAsync(Guid userId)
    {
        User? user = await RepositoryWrapper.Users.Find(x => x.Id == userId)
        .Include(x => x.UserPermissions)!
        .ThenInclude(x => x.Permission)
        .FirstOrDefaultAsync();
        if (user == null) return (null, Messages.Users.IdNotFound);
        user.Avatar = FileService.GetFullPath(user.Avatar);
        return (user, null);
    }

    public async Task<(User? User, string? ErrorMessage)> GetProfileAsync(Guid userId)
    {
        User? user = await RepositoryWrapper.Users.Find(x => x.Id == userId)
        .Include(x => x.UserPermissions)!
        .ThenInclude(x => x.Permission)
        .FirstOrDefaultAsync();
        if (user == null) return (null, Messages.Users.IdNotFound);
        user.Avatar = FileService.GetFullPath(user.Avatar);
        return (user, null);
    }

    private Task<string> TokenGenerate(string secretKey, string isUser, string isAudience, DateTime? expireTime, IEnumerable<Claim> claims = null!)
    {
        SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(isUser,
            isAudience,
            claims,
            expires: expireTime,
            signingCredentials: creds);
        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    private async Task<string> GenerateAccessTokenAsync(User user)
    {
        List<Claim> claims = new()
        {
            new Claim(JwtClaimsName.Identification, user.Id.ToString()),
            new Claim(JwtClaimsName.UserName, user.UserName!),
            new Claim(JwtClaimsName.EmailAddress, user.EmailAddress!),
        };
        if (user.UserPermissions != null && user.UserPermissions.Any())
        {
            List<Claim> permissionClaims = new();
            IEnumerable<string>? permissions = user.UserPermissions.Select(x=> x.Code!);
            foreach (string permission in permissions)
            {
                permissionClaims.Add(new Claim(JwtClaimsName.Roles, permission));
            }
            claims.AddRange(permissionClaims);
        }
        if (user.Avatar is not null)
        {
            claims.Add(new Claim(JwtClaimsName.Avatar, user.Avatar));
        }
        double timeExpire = double.Parse(Configuration["JwtSettings:ExpiredTime"]);

        return await TokenGenerate(Configuration["JwtSettings:SecretKey"],
            Configuration["JwtSettings:Issuer"],
            Configuration["JwtSettings:Issuer"],
            DateTime.UtcNow.AddMinutes(timeExpire),
            claims);
    }

    private async Task<string> GenerateRefreshTokenAsync()
    {
        double timeExpire;
        bool ok = double.TryParse(Configuration["JwtSettings:RefreshTokenExpire"], out timeExpire);
        if (!ok) throw new Exception("Check appsettings.json -> JwtSettings");

        return await TokenGenerate(Configuration["JwtSettings:RefreshKey"],
            Configuration["JwtSettings:Issuer"],
            Configuration["JwtSettings:Issuer"],
            DateTime.UtcNow.AddDays(double.Parse(Configuration["RefreshTokenExpiredTime"]))
            );
    }
    public async Task<(string? AccesToken, string? ErrorMessage)> AuthenticateAsync(UserSignInRequest request)
    {
        User? user = await RepositoryWrapper.Users
        .Find(x => x.UserName == request.UserName || x.EmailAddress == request.UserName)
        .Include(x => x.UserPermissions).FirstOrDefaultAsync();
        if(user == null) return(null, Messages.Users.NotFound);
        bool success = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!success) return (null, Messages.Users.PasswordIsWrong);

        string token = await GenerateAccessTokenAsync(user);
        return (token, null);
    }

    public async Task<User> CreateAsync(UserSignUpRequest request)
    {
        User user = Mapper.Map<User>(request);
        if (request.Avatar != null)
        {
            string? path = await FileService.UploadFileAsync(request.Avatar, "Avatar");
            user.Avatar = path;
        }
        List<UserPermission> rolePermissions = await RepositoryWrapper.RolePermissions.Find(x => x.RoleId == request.RoleId)
                .Select(x => new UserPermission() { UserId = user.Id, Code = x.Code }).ToListAsync();
        user.UserPermissions = new();
        user.UserPermissions.AddRange(rolePermissions);
        await RepositoryWrapper.Users.AddAsync(user);
        return user;
    }

    public async Task<PaginationResponse<User>?> GetAllAsync(PaginationRequest request)
    {
        IQueryable<User>? users = RepositoryWrapper.Users.Find(x =>
      (
          string.IsNullOrEmpty(request.Search)
          || x.UserName!.ToLower().Contains(request.Search!.ToLower())
          || x.EmailAddress!.ToLower().Contains(request.Search!.ToLower())
      ));
        await users.ForEachAsync(user =>
        {
            user.Avatar = FileService.GetFullPath(user.Avatar);
        });
        users = SortUtility<User>.ApplySort(users, request.OrderByQuery!);
        PaginationUtility<User>? data = await PaginationUtility<User>.ToPagedListAsync(users, request.Current, request.PageSize);
        return PaginationResponse<User>.PaginationInfo(data, data.PageInfo);
    }

    public async Task<(User User, string? ErrorMessage)> UpdateAsync(Guid userId, UpdateUserRequest request)
    {
        User user = (await RepositoryWrapper.Users.GetByIdAsync(userId))!;
        if (request.Password != null)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        if (request.Avatar != null)
        {
            string? path = await FileService.UpdateFileAsync(request.Avatar!, "Avatar", user.Avatar ?? null);
            user.Avatar = path;
        }
        if (user.RoleId != request.RoleId)
        {
            var oldUserRolePermissions = await RepositoryWrapper.RolePermissions
                .Find(x => x.RoleId == user.RoleId)
                .Select(x => new UserPermission { UserId = user.Id, Code = x.Code })
                .ToListAsync();
            var newUserRolePermissions = await RepositoryWrapper.RolePermissions
                .Find(x => x.RoleId == request.RoleId)
                .Select(x => new UserPermission { UserId = user.Id, Code = x.Code })
                .ToListAsync();
            await RepositoryWrapper.BeginTransactionAsync();
            try
            {
                await RepositoryWrapper.UserPermissions.DeleteRangeAsync(oldUserRolePermissions);
                await RepositoryWrapper.UserPermissions.AddRangeAsync(newUserRolePermissions);

                await RepositoryWrapper.CommitTransactionAsync();
            }
            catch (System.Exception ex)
            {
                await RepositoryWrapper.RollbackTransactionAsync();
                Log.Error(ex, ex.GetBaseException().ToString());
                return (user, Messages.Users.UpdateFailed);
            }
        }
        Mapper.Map(request, user);
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, null);
    }

    public async Task<string?> DeleteAsync(Guid userId)
    {
        User? user = await GetByIdAsync(userId);
        if(user == null) return (Messages.Users.IdNotFound);
        if (user.Avatar != null)
        {
            await FileService.DeleteFileAsync(user.Avatar);
        }
        await RepositoryWrapper.Users.DeleteAsync(user);
        return null;
    }

    public async Task<(User? User, string? ErrorMessage)> ResetPasswordAsync(Guid userId, ResetPasswordRequest request)
    {
        User? user = await GetByIdAsync(userId);
        if(user == null) return (null, Messages.Users.IdNotFound);

        user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        await RepositoryWrapper.Users.UpdateAsync(user);
        return (user, null);
    }

    public async Task<string?> UpdateProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        User? user = await GetByIdAsync(userId);
        if(user == null) return Messages.Users.IdNotFound;

        Mapper.Map(request, user);
        if (request.Password != null)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        if (request.Avatar != null)
        {
            string? path = await FileService.UpdateFileAsync(request.Avatar!, "Avatar", user.Avatar ?? null);
            user.Avatar = path;
        }
        await RepositoryWrapper.Users.UpdateAsync(user);
        return null;
    }

    public async Task<(List<Permission>? Permissions, string? ErrorMessage)> GetPermissionByUserAsync(Guid userId)
    {
        User? user = await GetByIdAsync(userId);
        if(user == null) return (null, Messages.Users.IdNotFound);
        
        List<Permission>? permissions = await RepositoryWrapper.UserPermissions
        .Find(x => x.UserId == user.Id)
        .Include(x => x.Permission)
        .Select(x => x.Permission!).ToListAsync();
        return (permissions, null);
    }

    public async Task AddPermissionByUserAsync(Guid userId, List<CreateUserPermissionRequest> request)
    {
        List<UserPermission> newUserRolePermissions = Mapper.Map<List<UserPermission>>(request);
        newUserRolePermissions.ForEach(x => x.UserId = userId);
        await RepositoryWrapper.UserPermissions.AddRangeAsync(newUserRolePermissions);
    }

    public async Task UpdatePermissionByUserAsync(Guid userId, List<UpdateUserPermissionRequest> request)
    {
        var entities = await RepositoryWrapper.UserPermissions.Find(x => x.UserId == userId, isAsNoTracking: true).ToListAsync();
        List<UserPermission>? keepUserPermissions = entities.Where(x => x.UserId == userId && request.Any(r => r.Code == x.Code)).ToList();
        List<UserPermission>? deleteUserPermissions = entities.Where(x => !keepUserPermissions.Contains(x)).ToList();

        var addUserPermissions = request.Where(r => !keepUserPermissions.Any(k => k.Code == r.Code));
        List<UserPermission>? newUserPermissions = Mapper.Map<List<UserPermission>>(addUserPermissions);
        newUserPermissions.ForEach(x => x.UserId = userId);

        Mapper.Map(request, keepUserPermissions);
        keepUserPermissions.ForEach(x => x.UserId = userId);

        await RepositoryWrapper.UserPermissions.AddRangeAsync(newUserPermissions);
        await RepositoryWrapper.UserPermissions.UpdateRangeAsync(keepUserPermissions);
        await RepositoryWrapper.UserPermissions.DeleteRangeAsync(deleteUserPermissions);
    }
}