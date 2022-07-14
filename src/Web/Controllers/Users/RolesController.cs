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
    public class RolesController : BaseController
    {
        private readonly IRoleService RoleService;

        public RolesController(IRoleService roleService)
        {
            RoleService = roleService;
        }
        /// <summary>
        /// get role detail
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        [HttpGet("{roleId}")]
        [Authorize(Roles = Permissions.Roles.View)]
        public async Task<IActionResult> DetailAsync(Guid roleId)
        {
            (Role? role, string? errorMessage) = await RoleService.GetDetailAsync(roleId);
            if (errorMessage != null) return BadRequest(errorMessage);
            return Ok(role, Messages.Roles.GetDetailSuccessfully);
        }

        /// <summary>
        /// Get All Roles
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationRequest request)
        {
            PaginationResponse<Role>? roles = await RoleService.GetAllAsync(request);
            return Ok(roles, Messages.Roles.GetAllSuccessfully);
        }

        /// <summary>
        /// Create Role
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Permissions.Roles.Create)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRoleRequest request)
        {
            await RoleService.CreateAsync(request);
            return Ok(null, Messages.Roles.CreateSuccessfully);
        }

        /// <summary>
        /// Get All Permissions
        /// </summary>
        [HttpGet("{roleId}/Permissions")]
        [Authorize(Roles = Permissions.Roles.View)]
        public async Task<IActionResult> GetAllPermissionAsync(Guid roleId)
        {
            List<Permission> permissions = await RoleService.GetAllPermissionByRoleAsync(roleId);
            return Ok(permissions, Messages.RolePermissions.GetAllSuccessfully);
        }

        /// <summary>
        /// Update Role
        /// </summary>
        [HttpPut("{roleId}")]
        [Authorize(Roles = Permissions.Roles.Edit)]
        public async Task<IActionResult> UpdateAsync(Guid roleId, [FromBody] UpdateRoleRequest request)
        {
            string? errorMessage = await RoleService.UpdateAsync(roleId, request);
            if(errorMessage != null) return BadRequest(errorMessage);
            return Ok(null, Messages.Roles.UpdateSuccessfully);
        }

        /// <summary>
        /// Delete Role
        /// </summary>
        [HttpDelete("{roleId}")]
        [Authorize(Roles = Permissions.Roles.Delete)]
        public async Task<IActionResult> DeleteAsync(Guid roleId)
        {
            string? errorMessage = await RoleService.DeleteAsync(roleId);
            if (errorMessage != null) return BadRequest(errorMessage);
            return Ok(null, Messages.Roles.DeleteSuccessfully);
        }
    }
}