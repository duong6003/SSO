using Infrastructure.Definitions;
using Infrastructure.Modules.Users;
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
    public class PermissionsController : BaseController
    {
        private readonly IPermissionService PermissionService;

        public PermissionsController(IPermissionService permissionService)
        {
            PermissionService = permissionService;
        }

        /// <summary>
        /// Get All Permission
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
        {
            PaginationResponse<Permission>? permissions = await PermissionService.GetAllAsync(request);
            return Ok(permissions, Messages.Permissions.GetAllSuccessfully);
        }
        /// <summary>
        /// Get All Permission By Name
        /// </summary>
        [HttpGet("GroupByName")]
        public async Task<IActionResult> GetAllGroupByNameAsync([FromQuery]PaginationRequest request)
        {
            PaginationResponse<GetPermissionResponse>? permissions = await PermissionService.GetAllGroupByNameAsync(request);
            return Ok(permissions, Messages.Permissions.GetAllSuccessfully);
        }

        /// <summary>
        /// Create Permission
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> CreateAsync([FromBody] CreatePermissionRequest request)
        {
            await PermissionService.CreateAsync(request);
            return Ok(null, Messages.Permissions.CreateSuccessfully);
        }

        /// <summary>
        /// Update Permission
        /// </summary>
        [HttpPut("{permissionCode}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateAsync(string permissionCode, [FromBody] UpdatePermissionRequest request)
        {
            await PermissionService.UpdateAsync(permissionCode, request);
            return Ok(null, Messages.Permissions.UpdateSuccessfully);
        }

        /// <summary>
        /// Delete Permission
        /// </summary>
        [HttpDelete("{permissionCode}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> DeleteAsync(string permissionCode)
        {
            await PermissionService.DeleteAsync(permissionCode);
            return Ok(null, Messages.Permissions.DeleteSuccessfully);
        }
    }
}