using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.Modules.Accounts.Services;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Accounts.Controllers
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
        [HttpGet()]
        public async Task<IActionResult> GetAll()
        {
            List<Permission> permissions = await PermissionService.GetAll();
            return ResponseOk(permissions, "GetPermissionsSuccess");
        }
    }
}
