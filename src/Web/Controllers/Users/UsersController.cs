using System.Security.Claims;
using Hangfire;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Modules.Users.Requests;
using Infrastructure.Modules.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Utilities;
namespace Web.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService UserService;

        public UsersController(IUserService userService)
        {
            UserService = userService;
        }
        /// <summary>
        /// Get detail information for user include user permission
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<IActionResult> DetailAsync(Guid userId)
        {
            (User? user, string? errorMessage) = await UserService.GetDetailAsync(userId);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(user, Messages.Users.GetDetailSuccessfully);
        }

        /// <summary>
        /// Get All Users
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Permissions.Users.View)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
        {
            PaginationResponse<User>? users = await UserService.GetAllAsync(request);
            return Ok(users, Messages.Users.GetAllSuccessfully);
        }

        /// <summary>
        /// Update user and user permission for admin, if user permission is not uploaded, it will be deleted
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request">
        /// Active : 0 -> InActive, 1: Active
        /// </param>
        /// <returns></returns>
        [HttpPut("{userId}")]
        [Authorize(Roles = Permissions.Users.Edit)]
        public async Task<IActionResult> UpdateAsync(Guid userId, [FromForm] UpdateUserRequest request)
        {
            (User user, string? errorMessage) = await UserService.UpdateAsync(userId, request);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(null, Messages.Users.UpdateSuccessfully);
        }

        /// <summary>
        /// delete users and related relations
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("{userId}")]
        [Authorize(Roles = Permissions.Users.Delete)]
        public async Task<IActionResult> DeleteAsync(Guid userId)
        {
            string? errorMessage = await UserService.DeleteAsync(userId);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(null, Messages.Users.DeleteSuccessfully);
        }

        /// <summary>
        /// get permission for user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/Permissions")]
        [Authorize(Roles = Permissions.Users.View)]
        public async Task<IActionResult> GetPermissionByUserAsync(Guid userId)
        {
            (List<Permission>? permissions, string? errorMessage) = await UserService.GetPermissionByUserAsync(userId);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(permissions, Messages.Permissions.GetAllSuccessfully);
        }

        /// <summary>
        /// add user permission for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{userId}/Permissions")]
        [Authorize(Roles = Permissions.Users.Create)]
        public async Task<IActionResult> AddPermissionByUserAsync(Guid userId, [FromBody] List<CreateUserPermissionRequest> request)
        {
            await UserService.AddPermissionByUserAsync(userId, request);
            return Ok(null, Messages.UserPermissions.CreateSuccessfully);
        }

        /// <summary>
        /// update permission for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{userId}/Products")]
        [Authorize(Roles = Permissions.Users.Edit)]
        public async Task<IActionResult> UpdatePermissionByUserAsync(Guid userId, List<UpdateUserPermissionRequest> request)
        {
            await UserService.UpdatePermissionByUserAsync(userId, request);
            return Ok(null, Messages.UserPermissions.UpdateSuccessfully);
        }

        /// <summary>
        /// change password for user
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("ChangePassword")]
        [Authorize(Roles = Permissions.Users.Edit)]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] ResetPasswordRequest request)
        {
            Guid userId = Guid.Parse(User.FindFirst(JwtClaimsName.Identification)!.Value);
            (User? user, string? errorMessage) = await UserService.ResetPasswordAsync(userId, request);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(null, Messages.Users.ResetPasswordSuccesfully);
        }

        /// <summary>
        /// get user profile information
        /// </summary>
        /// <returns></returns>
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            Guid userId = Guid.Parse(User.FindFirst(JwtClaimsName.Identification)!.Value);
            (User? user, string? errorMessage) = await UserService.GetProfileAsync(userId);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(user, Messages.Users.GetProfileSuccessfully);
        }

        /// <summary>
        /// Update user and user permission for user, if user permission is not uploaded, it will be deleted
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateProfileAsync([FromForm] UpdateProfileRequest request)
        {
            Guid userId = Guid.Parse(User.FindFirstValue(JwtClaimsName.Identification));
            string? errorMessage = await UserService.UpdateProfileAsync(userId, request);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(null, Messages.Users.UpdateSuccessfully);
        }

        /// <summary>
        /// sign up with username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignUp")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUpAsync([FromForm] UserSignUpRequest request)
        {
            User user = await UserService.CreateAsync(request);
            return Ok(null, Messages.Users.CreateSuccessfully);
        }

        /// <summary>
        /// sign in with username and password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInAsync([FromBody] UserSignInRequest request)
        {
            (string? AccesToken, string? errorMessage) = await UserService.AuthenticateAsync(request);
            if (errorMessage is not null) return BadRequest(errorMessage);
            return Ok(AccesToken, Messages.Users.SignInSuccess);
        }
    }
}