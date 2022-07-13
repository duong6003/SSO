using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Utilities;
using Project.Modules.Accounts.Entities;
using Project.Modules.Applications.Entities;
using Project.Modules.Applications.Requests;
using Project.Modules.Applications.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Modules.Applications.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApplicationsController : BaseController
    {
        private readonly IApplicationService ApplicationService;

        public ApplicationsController(IApplicationService applicationService)
        {
            ApplicationService = applicationService;
        }
        /// <summary>
        /// Register application
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "APPLICATION_ADD")]
        public async Task<IActionResult> RegisterAsync([FromForm]ApplicationRegisterRequest request)
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            (Application application, string message) = await ApplicationService.RegisterAsync(request, accountId);
            return ResponseOk(application, message);
        }
        /// <summary>
        /// Get secret key a application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpGet("{applicationId}/SecretKey")]
        public async Task<IActionResult> GetSecretKeyAsync(string applicationId)
        {
            Application application = await ApplicationService.GetByIdAsync(applicationId);
            if(application is null)
            {
                return ResponseBadRequest("ApplicationIdInValid");
            }
            return ResponseOk(application.ApplicationSecret, "GetSecretKeyApplicationSuccess");
        }
        /// <summary>
        /// Delete a Application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpDelete("{applicationId}")]
        [Authorize(Roles ="APPLICATION_DELETE")]
        public async Task<IActionResult> DeleteAsync(string applicationId)
        {
            Application application = await ApplicationService.GetByIdAsync(applicationId);
            if (applicationId is null)
            {
                return ResponseBadRequest("ApplicationIdInValid");
            }
            await ApplicationService.DeleteAsync(application);
            return ResponseOk(null, "DeleteApplicationSuccess");
        }
        /// <summary>
        /// Get List Account Of Application
        /// </summary>
        /// <param name="request"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpGet("{applicationId}/Accounts")]
        [Authorize(Roles = "ACCOUNT_APPLICATION_SHOW")]
        public async Task<IActionResult> GetByApplicationIdAsync([FromQuery]GetAllAccountApplicationRequest request, string applicationId)
        {
            PaginationResponse<Account> paginationResponse = await ApplicationService.GetByApplicationIdAsync(request,applicationId);
            return ResponseOk(paginationResponse, "GetAccountsSuccess");
        }
        /// <summary>
        /// Update Infomation Application
        /// </summary>
        /// <param name="request"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpPut("{applicationId}")]
        [Authorize(Roles = "APPLICATION_UPDATE")]
        public async Task<IActionResult> UpdateAsync([FromForm] ApplicationUpdateRequest request, string applicationId)
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            (Application application, string message) = await ApplicationService.UpdateAsync(request, applicationId);
            return ResponseOk(application, message);
        }
        /// <summary>
        /// Get new SecretKey a application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpGet("GetNewSecretKey/{applicationId}")]
        public async Task<IActionResult> GetNewSecretKeyAsync(string applicationId)
        {
            Application application = await ApplicationService.GetByIdAsync(applicationId);
            if(application is null)
            {
                return ResponseBadRequest("ApplicationIdInValid");
            }
            return ResponseOk(await ApplicationService.GetNewSecretKeyAsync(applicationId), "GetNewSecretKeyApplicationSuccess");
        }
        /// <summary>
        /// Update status application
        /// Locked: 0
        /// Active: 1
        /// </summary>
        /// <param name="request"></param>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpPut("{ApplicationId}/UpdateStatus")]
        [Authorize(Roles ="APPLICATION_UPDATE")]
        public async Task<IActionResult> UpdateAsync([FromBody] ApplicationUpdateStatusRequest request, string applicationId)
        {
            Application application = await ApplicationService.UpdateStatusAsync(request, applicationId);
            return ResponseOk(application, "UpdateApplicationStatusSuccess");
        }
        
        /// <summary>
        /// Get All Application
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "APPLICATION_SHOW")]
        public async Task<IActionResult> GetAllAsync()
        {
            List<Application> applications = await ApplicationService.GetAll();
            return ResponseOk(applications, "GetApplicationsSuccess");
        }
        /// <summary>
        /// Get detail a Application
        /// </summary>
        /// <param name="applicationId"></param>
        /// <returns></returns>
        [HttpGet("{applicationId}")]
        [Authorize(Roles ="APPLICATION_DETAIL_SHOW")]
        public async Task<IActionResult> GetDetailAsync(string applicationId)
        {
            Application application = await ApplicationService.GetDetailAsync(applicationId);
            return ResponseOk(application, "GetApplicationSuccess");
        }

        [HttpGet("AllowedAccess")]
        [Authorize]
        public async Task<IActionResult> AllowedAccessAsync()
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Application> applications = await ApplicationService.AllowedAccessAsync(accountId);
            return ResponseOk(applications, "GetAppsAllowedAccessSuccess");
        }
    }
}
