using AutoMapper;
using Core.Common.Interfaces;
using Core.Utilities;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.Modules.Users.Services
{
    public class RolePermissionCompare : IEqualityComparer<RolePermission>
    {
        public bool Equals(RolePermission? x, RolePermission? y)
        {
            return x!.RoleId == y!.RoleId && x.Code == y.Code;
        }

        public int GetHashCode([DisallowNull] RolePermission obj)
        {
            unchecked
            {
                if (obj == null)
                    return 0;
                return obj.GetHashCode();
            }
        }
    }

    public interface IRoleService : IScopedService
    {
        Task<Role?> GetByIdAsync(Guid roleId);

        Task<(Role? Role, string? ErrorMessage)> GetDetailAsync(Guid roleId);

        Task<PaginationResponse<Role>?> GetAllAsync(PaginationRequest request);

        Task CreateAsync(CreateRoleRequest request);

        Task<string?> UpdateAsync(Guid roleId, UpdateRoleRequest request);

        Task<string?> DeleteAsync(Guid roleid);

        Task<List<Permission>> GetAllPermissionByRoleAsync(Guid roleId);
    }

    public class RoleService : IRoleService
    {
        private readonly IRepositoryWrapper RepositoryWrapper;
        private readonly IMapper Mapper;

        public RoleService(IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
        }

        public async Task CreateAsync(CreateRoleRequest request)
        {
            Role role = Mapper.Map<Role>(request);
            await RepositoryWrapper.Roles.AddAsync(role);
        }

        public async Task<string?> DeleteAsync(Guid roleId)
        {
            Role? role = await RepositoryWrapper.Roles.Find(x => x.Id == roleId).Include(x => x.Users).FirstOrDefaultAsync();
            if (role == null) return (Messages.Roles.IdNotFound);
            if (role.Users!.Any()) return (Messages.Roles.BeingUsed);
            await RepositoryWrapper.Roles.DeleteAsync(role);
            return null;
        }

        public async Task<PaginationResponse<Role>?> GetAllAsync(PaginationRequest request)
        {
            IQueryable<Role>? roles = RepositoryWrapper.Roles.Find(x =>
                (
                    string.IsNullOrEmpty(request.Search)
                    || x.Name!.ToLower().Contains(request.Search!.ToLower())
                ));
            roles = SortUtility<Role>.ApplySort(roles, request.OrderByQuery!);
            PaginationUtility<Role>? data = await PaginationUtility<Role>.ToPagedListAsync(roles, request.Current, request.PageSize);
            return PaginationResponse<Role>.PaginationInfo(data, data.PageInfo);
        }

        public async Task<List<Permission>> GetAllPermissionByRoleAsync(Guid roleId)
        {
            List<Permission> permissions = await RepositoryWrapper.RolePermissions
                .Find(x => x.RoleId == roleId)
                .Include(x => x.Permission)
                .Select(x => x.Permission!).ToListAsync();
            return permissions;
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            return await RepositoryWrapper.Roles.GetByIdAsync(roleId);
        }

        public async Task<(Role? Role, string? ErrorMessage)> GetDetailAsync(Guid roleId)
        {
            Role? role = await RepositoryWrapper.Roles.Find(x => x.Id == roleId)
                .Include(x => x.RolePermissions)!
                .ThenInclude(x => x.Permission)
                .FirstOrDefaultAsync();
            if (role == null) return (null, Messages.Roles.IdNotFound);
            return (role, null);
        }

        private Task<List<RolePermission>> GetDeleteRolePermission(List<RolePermission> sources, List<RolePermission> excepts)
        {
            var comparer = new RolePermissionCompare();
            List<RolePermission> results = new();
            sources.ForEach(source =>
            {
                if (!excepts.Any(except => comparer.Equals(except, source)))
                {
                    results.Add(source);
                }
            });
            return Task.FromResult(results);
        }

        public async Task<string?> UpdateAsync(Guid roleId, UpdateRoleRequest request)
        {
            Role? role = await GetByIdAsync(roleId);
            if (request.RolePermissions != null)
            {
                if (request.RolePermissions.Any())
                {
                    string? errorMessage = await UpdatePermisstionByRoleAsync(roleId, request.RolePermissions);
                    if (errorMessage != null) return errorMessage;
                }
                else
                {
                    await RepositoryWrapper.RolePermissions.DeleteRangeAsync(RepositoryWrapper.RolePermissions.Find(x => x.RoleId == roleId));
                }
            }

            Mapper.Map(request, role);
            await RepositoryWrapper.Roles.UpdateAsync(role!);
            return null;
        }

        private async Task<string?> UpdatePermisstionByRoleAsync(Guid roleId, List<UpdateRolePermissionRequest> request)
        {
            await RepositoryWrapper.BeginTransactionAsync();
            try
            {
                var entities = RepositoryWrapper.RolePermissions.Find(x => x.RoleId == roleId, isAsNoTracking: true);
                List<RolePermission>? keepRolePermissions = await entities.Where(x => request.Select(r => r.Code).Contains(x.Code)).ToListAsync();

                var addRolePermissionRequests = request.Where(r => !keepRolePermissions.Exists(k => k.Code == r.Code));
                var updateRolePermissionRequests = request.Where(r => keepRolePermissions.Exists(k => k.Code == r.Code)).ToList();

                List<RolePermission>? newRolePermissions = Mapper.Map<List<RolePermission>>(addRolePermissionRequests);
                newRolePermissions.ForEach(x => x.RoleId = roleId);
                List<RolePermission>? deleteRolePermissions = await entities.Where(x => !updateRolePermissionRequests.Select(u => u.Code).Contains(x.Code)).ToListAsync();

                Mapper.Map(updateRolePermissionRequests, keepRolePermissions);
                keepRolePermissions.ForEach(x => x.RoleId = roleId);

                await RepositoryWrapper.RolePermissions.AddRangeAsync(newRolePermissions);
                await RepositoryWrapper.RolePermissions.DeleteRangeAsync(deleteRolePermissions);
                await RepositoryWrapper.RolePermissions.UpdateRangeAsync(keepRolePermissions);

                List<User> users = await RepositoryWrapper.Users.Find(x => x.RoleId == roleId).Include(x => x.UserPermissions).ToListAsync();
                users.ForEach(async user =>
                {
                    IEnumerable<UserPermission> addUserPermissionRequests = newRolePermissions.Select(x => new UserPermission() { UserId = user.Id, Code = x.Code });
                    IEnumerable<UserPermission> deleteUserPermissionRequests = deleteRolePermissions.Select(x => new UserPermission() { UserId = user.Id, Code = x.Code });
                    await RepositoryWrapper.UserPermissions.AddRangeAsync(addUserPermissionRequests);
                    await RepositoryWrapper.UserPermissions.DeleteRangeAsync(deleteUserPermissionRequests);
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "-------->" + ex.GetBaseException().ToString());
                await RepositoryWrapper.RollbackTransactionAsync();
                return Messages.Roles.UpdateFailed;
            }
            await RepositoryWrapper.CommitTransactionAsync();
            return null;
        }
    }
}