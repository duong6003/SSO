using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Utilities;
using Project.Modules.Accounts.Entities;
using Project.Modules.Accounts.Requests;
using Project.Modules.Accounts.Responses;
using Project.Modules.Accounts.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Modules.Accounts.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController : BaseController
    {
        private readonly IAccountService AccountService;

        public AccountsController(IAccountService accountService)
        {
            AccountService = accountService;
        }

        /// <summary>
        /// Register normal Account 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync([FromForm] AccountRegisterRequest request)
        {
            (Account account, string message) = await AccountService.RegisterAsync(request);
            return ResponseOk(account, message);
        }
        /// <summary>
        /// Add a Account SSO
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "ACCOUNT_ADD, ACCOUNT_SSO_ADD")]
        public async Task<IActionResult> StoreAsync([FromForm] AccountStoreRequest request)
        {
            (Account account, string message) = await AccountService.StoreAsync(request);
            return ResponseOk(account, message);
        }
        /// <summary>
        /// Login via SSO
        /// Account normal : Redirect to application
        /// Account SSO : Login into SSO System
        /// </summary>
        /// <param name="request"></param>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync([FromBody] AccountLoginRequest request)
        {
            (object tokenInfo, string errorMessage) = await AccountService.LoginAsync(request);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                return ResponseBadRequest(errorMessage);
            }
            return ResponseOk(tokenInfo, "LoginAccountSuccess");
        }
        /// <summary>
        /// Verify OTP
        /// /// Account normal : Redirect to application
        /// Account SSO : Login into SSO System
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("VerifyOTP")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTPAsync([FromBody] AccountVeriryOTPRequest request)
        {
            object data = await AccountService.VerifyOTPAsync(request);
            return ResponseOk(data, "VerifyAccountOTPSuccess");
        }
        /// <summary>
        /// Get Detail Account SSO
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetDetailAsync(string accountId)
        {
            Account account = await AccountService.GetProfileAsync(accountId);
            if (account is null)
            {
                return ResponseBadRequest("AccountIdInValid");
            }
            return ResponseOk(account, "GetProfileAccountSuccess");
        }
        /// <summary>
        /// Update Account SSO
        /// Status Account : Active = 1, Deactive = 0
        /// Two-Factor Auth : On = 1, Off = 0
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPut("{accountId}")]
        [Authorize(Roles = "ACCOUNT_UPDATE, ACCOUNT_SSO_UPDATE")]
        public async Task<IActionResult> UpdateAsync([FromForm] AccountUpdateRequest request, string accountId)
        {
            string accountCurrennt = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            (Account account, string message) = await AccountService.UpdateAsync(request, accountId, accountCurrennt);
            if(account is null)
            {
                return ResponseForbidden(message);
            }
            return ResponseOk(account, message);
        }
        /// <summary>
        /// Update Two factor auth account normal
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accountUserName"></param>
        /// <returns></returns>
        [HttpPut("{accountUserName}/UpdateAuth")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateAsync([FromBody] AccountUpdateTwoFactorRequest request, string accountUserName)
        {
            Account account= await AccountService.UpdateTwoFactorAuthAsync(request, accountUserName);
            return ResponseOk(account, "UpdateAccountSuccess");
        }
        /// <summary>
        /// Update Profile Account 
        /// </summary>
        /// <returns></returns>
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfileAsync([FromForm] AccountUpdateProfileRequest request)
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            (Account account, string message) = await AccountService.UpdateProfileAsync(request, accountId);
            if(account is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(account, message);
        }
        /// <summary>
        /// Get Profile Account 
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetProfile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Account account = await AccountService.GetProfileAsync(accountId);
            return Ok(account);
        }
        /// <summary>
        /// Get List Account 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = "ACCOUNT_SSO_SHOW")]
        public async Task<IActionResult> GetAllAsync([FromQuery] GetAllAccountSSORequest request)
        {
            PaginationResponse<Account> paginationResponse = await AccountService.GetAllAsync(request);
            return ResponseOk(paginationResponse, "GetAllAccountSuccess");
        }
        /// <summary>
        /// Support Delete Many Account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("DeleteMany")]
        [Authorize(Roles = "ACCOUNT_DELETE, ACCOUNT_SSO_DELETE")]
        public async Task<IActionResult> DeleteManyAsync([FromBody] AccountDeleteRequest request)
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool delete = await AccountService.DeleteManyAsync(request, accountId);
            if (!delete)
            {
                return ResponseForbidden("DoNotPermissionWithAccountIds");
            }
            return ResponseOk(null, "DeleteAccountsSuccess");
        }
        /// <summary>
        /// Get new OTP Code if do not Receive OTP When using LoginWithOTP
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet("{accountId}/NewOTPCode")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNewOTPCodeAsync(string accountId)
        {
            Account account = await AccountService.GetByIdAsync(accountId);
            if (account is null)
            {
                return ResponseBadRequest("AccountIdInValid");
            }
            AccountOTPResponse accountOTPResponse = await AccountService.GetNewOTPCodeAsync(account);
            if (accountOTPResponse is null)
            {
                return ResponseBadRequest("PleaseReLogin");
            }
            return ResponseOk(accountOTPResponse, "GetNewOTPSuccess");
        }
        /// <summary>
        /// Get token by code
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Token")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTokenAsync([FromForm] GetTokenApplicationRequest request)
        {
            (object data, string message) = await AccountService.GetTokenAsync(request.Client_id ,request.Code);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return Ok(data);
        }
        /// <summary>
        /// Get Profile by accountId using applicationId
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetProfileById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProfileAccountIdByApplicationAsync([FromBody] GetProfileAccountIdApplicationRequest request)
        {
            Account account = await AccountService.GetProfileAccountByApplicationAsync(request.AccountId);
            return ResponseOk(account, "GetProfileAccountSuccess");
        }
        /// <summary>
        /// Change Password Account
        /// </summary>
        /// <param name="request"></param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] AccountChangePasswordRequest request)
        {
            string accountId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            bool changePassword = await AccountService.ChangePasswordAsync(request, accountId);
            if (!changePassword)
            {
                return ResponseBadRequest("PasswordIncorrect");
            }
            
            return ResponseOk(null, "ChangePasswordAccountSuccess");
        }
        /// <summary>
        /// Send Recovery link to Email Reset Account Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("SendPasswordRecoveryLink")]
        [AllowAnonymous]
        public async Task<IActionResult> SendPasswordRecoveryLinkAsync([FromBody] AccountResetPasswordRequest request)
        {
            await AccountService.SendPasswordRecoveryLinkAsync(request);
            return ResponseOk(null, "SendPasswordRecoveryLinkSuccess");
        }
        /// <summary>
        /// Check Recovery token Valid
        /// </summary>
        /// <param name="recoveryToken"></param>
        /// <returns></returns>
        [HttpGet("CheckRecoveryToken")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckRecoveryTokenAsync([FromQuery] int recoveryToken)
        {
            RecoveryTokens recovery = await AccountService.CheckRecoveryTokenAsync(recoveryToken);
            if (recovery is null)
            {
                return ResponseBadRequest("RecoveryTokenInvalid");
            }
            return ResponseOk(null, "CheckRecoveryTokenAccountSuccess");
        }
        /// <summary>
        /// Reset Password When Check recovery token Valid
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> PasswordResetAsync([FromBody] AccountResetNewPasswordRequest request)
        {
            if (await AccountService.ResetPasswordAsync(request))
            {
                return ResponseOk(null, "ResetPasswordSuccess");
            }
            return ResponseBadRequest("RecoveryTokenInvalid");
        }
        //[HttpPost("RefreshToken")]
        //[AllowAnonymous]
        //public async Task<IActionResult> RefreshTokenAsync([FromBody] AccountGetRefreshTokenRequest request)
        //{
        //    (AccountGetRefreshTokenResponse accountGetRefreshTokenResponse, string message) = await AccountService.RefreshTokenAsync(request);
        //    if (accountGetRefreshTokenResponse is null)
        //    {
        //        return ResponseBadRequest(message);
        //    }
        //    return ResponseOk(accountGetRefreshTokenResponse, message);
        //}
    }
}
