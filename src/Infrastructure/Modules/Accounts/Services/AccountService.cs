using Amazon.S3;
using AutoMapper;
using Core.Ultilities;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Infrastructure.Modules.Accounts.Entities;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Responses;
using Infrastructure.Modules.Applications.Entities;
using Infrastructure.Modules.Users.Entities;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Common.Interfaces;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Definitions;

namespace Infrastructure.Modules.Accounts.Services
{
    public interface IAccountService : IScopedService
    {
        Task<(Account account, string message)> RegisterAsync(AccountRegisterRequest request);
        Task<(object TokenInfo, string ErrorMessage)> LoginAsync(AccountLoginRequest request);
        Task<bool> IsAccountUserNameActiveValidationAsync(string accountUserName);
        Task<object> VerifyOTPAsync(AccountVeriryOTPRequest request);
        Task<(Account, string)> UpdateProfileAsync(AccountUpdateProfileRequest request, string accountId);
        Task<Account> GetByIdAsync(string accountId);
        Task<(object, string)> GetTokenAsync(string applicationId, string code);
        Task<(Account, string)> UpdateAsync(AccountUpdateRequest request, string accountUpdateId, string accountId);
        Task<bool> ChangePasswordAsync(AccountChangePasswordRequest request, string accountId);
        Task<(Account account, string message)> StoreAsync(AccountStoreRequest request);
        Task<bool> IsAccountPhoneDoesNotExistValidationAsync(string accountPhone);
        Task<PaginationResponse<Account>> GetAllAsync(GetAllAccountSSORequest request);
        Task<bool> DeleteManyAsync(AccountDeleteRequest request, string accountId);
        Task<AccountOTPResponse> GetNewOTPCodeAsync(Account account);
        Task<Account> UpdateTwoFactorAuthAsync(AccountUpdateTwoFactorRequest request, string accountId);
        Task SendPasswordRecoveryLinkAsync(AccountResetPasswordRequest request);
        Task<bool> ResetPasswordAsync(AccountResetNewPasswordRequest request);
        Task<Account> GetProfileAsync(string accountId);
        Task<RecoveryToken> CheckRecoveryTokenAsync(int? recoveryToken);
        bool IsValidContentType(IFormFile accountAvatar, List<string> list);
        Task<(AccountGetRefreshTokenResponse, string)> RefreshTokenAsync(AccountGetRefreshTokenRequest request);
        Task<Account> GetProfileAccountByApplicationAsync(string accountId);

        #region Validation
        Task<bool> IsPasswordCorrectValidationAsync(string accountUserName, string accountPassword);
        Task<bool> IsCorrectOTPCodeValidationAsync(string accountId, string oTPCode);
        Task<bool> IsAccountUserNameExistsValidationAsync(string accountUserName);
        Task<bool> IsUserNameDoesNotExistsValidationAsync(string userName, string? accountId = null);
        Task<bool> IsEmailDoesNotExistValidationAsync(string accountEmail, string? accountId = null);
        Task<bool> IsIdentityCardDoesNotExistsValidationAsync(string accountIdentityCard, string? accountId = null);
        bool IsValidUrlValidation(string redirectUrl);
        Task<bool> IsPhoneDoesNotExistValidationAsync(string accountEmail, string? accountId = null);
        Task<bool> IsApplicationSecretValidValidationAsync(string applicationId, string applicationSecret);
        Task<bool> IsAccountStatusSameAvailable(string accountId, AccountStatus? accountStatus);
        Task<bool> IsPermissionCodesExistsValidationAsync(List<string> permissionCodes);
        Task<bool> IsAccountIdsExistsValidationAsync(List<string> accountIds);
        Task<bool> IsPhoneExistValidationAsync(string accountPhone, string? applicationId = null);
        Task<bool> IsCorrectTryNumberInputOTPAsync(string accountId, string oTPCode);
        Task<bool> IAccountPhoneActiveValidationAsync(string accountPhone);
        Task<bool> IsEmailExistValidationAsync(string accountEmail);
        Task<bool> IsAccountTwoFactorAuthSameAvailable(string accountId, TwoFactorAuth? accountTwoFactorAuth);
        Task<bool> IsAccountIdsSameAsType(List<string> accountIds);
        bool VerifyCaptchaAsync(string token);

        #endregion
    }
    public class AccountService : IAccountService
    {
        private readonly IRepositoryWrapper RepositoryWrapper;
        private readonly string AccountAvatar = "SSOAvatartImageS";
        private readonly IAmazonS3Utility AmazonS3Utility;
        private readonly IMapper Mapper;
        private readonly IDatabase RedisRepository;
        private readonly IConfiguration Configuration;

        public AccountService(IRepositoryWrapper repositoryWrapper, IAmazonS3Utility amazonS3Utility, IMapper mapper, IConfiguration configuration, IRedisDatabaseProvider redisDatabaseProvider)
        {
            RepositoryWrapper = repositoryWrapper;
            AmazonS3Utility = amazonS3Utility;
            Mapper = mapper;
            Configuration = configuration;
            RedisRepository = redisDatabaseProvider.GetDatabase();
        }
        public async Task<(object? TokenInfo, string ErrorMessage)> LoginAsync(AccountLoginRequest request)
        {
            Account? account = await RepositoryWrapper.Accounts.Find(x => x.UserName == request.UserName
                || x.PhoneNumber == request.UserName
            ).Include(x => x.AccountPermissions).FirstOrDefaultAsync();
            if (account == null) return (null, Messages.Accounts.NotFound);
            if (!(BCrypt.Net.BCrypt.Verify(request.Password, account.Password)))
            {
                if (!RedisRepository.KeyExists("CP" + account.Id))
                {
                    RedisRepository.StringSet("CP" + account.Id, 1, TimeSpan.FromSeconds(300));
                }
                else
                {
                    int n = int.Parse(RedisRepository.StringGet("CP" + account.Id)!);
                    if (n == int.Parse(Configuration["OTPLoginSettings:MaxWrongPassword"]))
                    {
                        account.AccountStatus = AccountStatus.Locked;
                        await RepositoryWrapper.Accounts.UpdateAsync(account);
                        RedisRepository.KeyDelete("CP" + account.Id);
                        return (null, Messages.Accounts.LockedEnteringWrongPasswordManyTime);
                    }
                    else
                    {
                        RedisRepository.StringSet("CP" + account.Id, n + 1);
                    }
                }
                return (null, "AccountPasswordInCorrect");
            }
            else
            {
                if (RedisRepository.KeyExists("CP" + account.Id))
                {
                    RedisRepository.KeyDelete("CP" + account.Id);
                }
            }

            if (account.TwoFactorAuth == TwoFactorAuth.Off)
            {
                account.LastAccess = DateTime.UtcNow;
                await RepositoryWrapper.Accounts.UpdateAsync(account);
                string accessToken = GenerateJwtToken(account);
                string code;
                if (request.SuccessUrl is null)
                {
                    if (account.AccountType == AccountType.SSO)
                    {
                        return (new { accessToken = accessToken }, null);
                    }
                    //AccountRefreshTokens accountRefreshTokens = await SaveRefreshToken(account, request);
                    //AccountLoginResponse accountLoginResponse;
                    if (request.ApplicationId is not null)
                    {
                        Application? application = await RepositoryWrapper.Applications.Find(x => x.Id.Equals(request.ApplicationId)).FirstOrDefaultAsync();
                        code = await GenerateCodeAsync(request, account.Id, application.Id); ;
                        //if (application.ApplicationRedirectUrl.Contains("?"))
                        //{
                        //    accountLoginResponse = new(null, int.Parse(Configuration["JwtSettings:ExpiredTime"]), $"{ application.ApplicationRedirectUrl }&code={code}");
                        //}
                        //accountLoginResponse = new(null, int.Parse(Configuration["JwtSettings:ExpiredTime"]), $"{ application.ApplicationRedirectUrl }?code={code}");
                        return (new { accessToken = accessToken, successUrl = $"{ application.ApplicationRedirectUrl }?code={code}" }, null);
                    }
                }
                code = await GenerateCodeAsync(request, account.AccountId, request.ApplicationId);
                if (!string.IsNullOrEmpty(request.SuccessUrl))
                {
                    if (request.SuccessUrl.Contains("?"))
                    {
                        request.SuccessUrl = $"{request.SuccessUrl}&code={code}";
                    }
                    else
                    {
                        request.SuccessUrl = $"{request.SuccessUrl}?code={code}";
                    }
                }
                return (new { accessToken = accessToken, successUrl = request.SuccessUrl }, null);
            }
            AccountOTPResponse accountOTPResponse = await LoginWithOTPAsync(account, request);
            return (accountOTPResponse, null);
        }
        private async Task<string> GenerateCodeAsync(AccountLoginRequest request, string accountId, string applicationId)
        {
            string code = Guid.NewGuid().ToString();
            await RedisRepository.SetRecordAsync(code + applicationId, accountId, TimeSpan.FromHours(int.Parse(Configuration["CodeSettings:ExpiredTime"])));
            return code;

        }
        public async Task<Account> GetProfileAccountByApplicationAsync(string accountId)
        {
            return await RepositoryWrapper.Accounts.Find(x => x.AccountId.Equals(accountId) && x.AccountType == AccountType.Normal).FirstOrDefaultAsync();
        }
        private async Task<AccountRefreshTokens> SaveRefreshToken(Account account, AccountLoginRequest request)
        {
            AccountRefreshTokens accountRefreshToken = await RepositoryWrapper.AccountRefreshTokens.Find(x => x.AccountId.Equals(account.AccountId)).FirstOrDefaultAsync();
            string refreshToken = "";
            if (accountRefreshToken is null)
            {
                refreshToken = GenerateRefreshToken();
                AccountRefreshTokens accountRefresh = new(request.ApplicationId, account.AccountId, refreshToken, DateTime.UtcNow.AddDays(int.Parse(Configuration["JwtSettings:RefreshTokenExpiredTime"])));
                RepositoryWrapper.AccountRefreshTokens.Add(accountRefresh);
            }
            else
            {
                if (accountRefreshToken.AccountRefreshTokenExpiredAt < DateTime.UtcNow)
                {
                    refreshToken = GenerateRefreshToken();
                    RepositoryWrapper.AccountRefreshTokens.Remove(accountRefreshToken);
                    AccountRefreshTokens accountRefresh = new(request.ApplicationId, account.AccountId, refreshToken, DateTime.UtcNow.AddDays(int.Parse(Configuration["JwtSettings:RefreshTokenExpiredTime"])));
                    RepositoryWrapper.AccountRefreshTokens.Add(accountRefresh);
                }
                refreshToken = accountRefreshToken.AccountRefreshToken;
            }
            await RepositoryWrapper.SaveChangesAsync();
            return accountRefreshToken;
        }
        public async Task<(AccountGetRefreshTokenResponse, string)> RefreshTokenAsync(AccountGetRefreshTokenRequest request)
        {
            (ClaimsPrincipal principal, string message) = GetPrincipalFromExpiredToken(request.OldToken);
            if (principal is null)
            {
                return (null, message);
            }
            string accountId = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            AccountRefreshTokens accountRefreshTokens = await RepositoryWrapper.AccountRefreshTokens.Find(x => x.AccountId.Equals(accountId)).FirstOrDefaultAsync();
            if (accountRefreshTokens is null && !accountRefreshTokens.AccountRefreshToken.Equals(request.RefreshToken))
            {
                return (null, "RefreshTokenInValid");
            }
            if (accountRefreshTokens.AccountRefreshTokenExpiredAt < DateTime.UtcNow)
            {
                return (null, "RefreshTokenExpired");
            }
            Account account = await GetByIdAsync(accountId);
            string newJwtToken = GenerateJwtToken(account);
            accountRefreshTokens.AccountRefreshToken = GenerateRefreshToken();
            RepositoryWrapper.AccountRefreshTokens.Update(accountRefreshTokens);
            await RepositoryWrapper.SaveChangesAsync();
            AccountGetRefreshTokenResponse accountGetRefreshTokenResponse = new(newJwtToken, int.Parse(Configuration["JwtSettings:ExpiredTime"]), accountRefreshTokens.AccountRefreshToken);
            return (accountGetRefreshTokenResponse, "GetRefreshTokenSuccess");
        }
        private (ClaimsPrincipal, string) GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters tokenValidationParameters = new()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"])),
                ValidateLifetime = false,
                ValidIssuer = Configuration["JwtSettings:Issuer"],
                ValidAudience = Configuration["JwtSettings:Issuer"],
            };
            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            SecurityToken securityToken;
            ClaimsPrincipal principal = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (securityToken.ValidTo.ToUniversalTime() > DateTime.UtcNow)
            {
                return (null, "OldTokenNotExpired");
            }
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return (null, "OldTokenInvalid");
            }
            return (principal, null);
        }
        public async Task<bool> ResetPasswordAsync(AccountResetNewPasswordRequest request)
        {
            RecoveryTokens recoveryTokens = await RedisRepository.GetRecordAsync<RecoveryTokens>(request.RecoveryToken.ToString());
            if (recoveryTokens is not null && recoveryTokens.RecoveryToken.Equals(request.RecoveryToken))
            {
                Account account = await GetByIdAsync(recoveryTokens.AccountId);
                account.AccountPassword = (request.AccountNewPassword + account.AccountSaft).HashPassword();
                RepositoryWrapper.Accounts.Update(account);
                await RepositoryWrapper.SaveChangesAsync();
                await RedisRepository.DeleteRecordAsync(recoveryTokens.RecoveryToken.ToString());
                return true;
            }
            return false;
        }
        public async Task<AccountOTPResponse> LoginWithOTPAsync(Account account, AccountLoginRequest accountLoginRequest)
        {
            AccountOTPResponse accountOTPResponse;
            accountOTPResponse = await GetOTPAsync(account);
            await RedisRepository.SetRecordAsync(account.AccountPhone, accountLoginRequest, TimeSpan.FromHours(1));
            return accountOTPResponse;
        }
        public async Task<Account> UpdateTwoFactorAuthAsync(AccountUpdateTwoFactorRequest request, string accountUserName)
        {
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountUserName.Equals(accountUserName)).FirstOrDefaultAsync();
            account.AccountTwoFactorAuth = request.AccountTwoFactorAuth;
            RepositoryWrapper.Accounts.Update(account);
            await RepositoryWrapper.SaveChangesAsync();
            return account;
        }
        public async Task<object> VerifyOTPAsync(AccountVeriryOTPRequest request)
        {
            Account account = await GetByIdAsync(request.AccountId);
            AccountLoginRequest accountLoginRequest = await RedisRepository.GetRecordAsync<AccountLoginRequest>(account.AccountPhone);
            account.AccountLastAccess = DateTime.UtcNow;
            RepositoryWrapper.Accounts.Update(account);
            await RepositoryWrapper.SaveChangesAsync();
            string jwtToken = GenerateJwtToken(account);
            await RedisRepository.DeleteRecordAsync(account.AccountPhone);
            await RedisRepository.DeleteRecordAsync(account.AccountId);
            string code;
            if (accountLoginRequest.SuccessUrl is null)
            {
                if (account.AccountType == AccountType.SSO)
                {
                    return new { accessToken = jwtToken };
                }
            }
            if (!string.IsNullOrEmpty(accountLoginRequest.SuccessUrl))
            {
                code = await GenerateCodeAsync(accountLoginRequest, account.AccountId, accountLoginRequest.ApplicationId);
                if (accountLoginRequest.SuccessUrl.Contains("?"))
                {
                    accountLoginRequest.SuccessUrl = $"{accountLoginRequest.SuccessUrl}&code={code}";
                }
                else
                {
                    accountLoginRequest.SuccessUrl = $"{accountLoginRequest.SuccessUrl}?code={code}";
                }
            }
            return new { accessToken = jwtToken, successUrl = accountLoginRequest.SuccessUrl };
        }
        public async Task<(object, string)> GetTokenAsync(string applicationId, string code)
        {
            string accountId = await RedisRepository.GetRecordAsync<string>(code + applicationId);
            if (accountId is null)
            {
                return (null, "CodeDoesNotExistOrAlreadyUsed");
            }
            if (code is null)
            {
                return (null, "CodeInValid");
            }
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountId.Equals(accountId)).FirstOrDefaultAsync();
            string jwtToken = GenerateJwtToken(account);
            await RedisRepository.DeleteRecordAsync(code + applicationId);
            AccountLoginResponse accountLoginResponse = new(jwtToken, int.Parse(Configuration["JwtSettings:ExpiredTime"]), null);
            return (accountLoginResponse, "GetTokenSuccess");
        }
        private string GenerateJwtToken(Account account)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, account.AccountId),
                new Claim("accountId", account.AccountId),
                new Claim("accountUserName", account.AccountUserName),
                new Claim("accountEmail", account.AccountEmail),
                new Claim("accountPhone", account.AccountPhone),
                new Claim("accountStatus", account.AccountStatus.ToString())
            };
            if (account.AccountType == AccountType.Normal)
            {
                JwtSecurityToken jwtSecurityToken = new
                (
                    issuer: Configuration["JwtSettings:Issuer"],
                    audience: Configuration["JwtSettings:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddSeconds(double.Parse(Configuration["JwtSettings:ExpiredTime"])),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"])), SecurityAlgorithms.HmacSha256)
                );
                return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            }
            else
            {
                foreach (AccountPermission accountPermission in account.AccountPermissions)
                {
                    claims.Add(new Claim(ClaimTypes.Role, accountPermission.PermissionCode));
                    claims.Add(new Claim("permissions", accountPermission.PermissionCode));
                }
                JwtSecurityToken jwtSecurityToken = new
                (
                    issuer: Configuration["JwtSettings:Issuer"],
                    audience: Configuration["JwtSettings:Issuer"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(double.Parse(Configuration["JwtSettings:SSOExpiredTime"])),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtSettings:SecretKey"])), SecurityAlgorithms.HmacSha256)
                );
                return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            }
        }
        public async Task<bool> ChangePasswordAsync(AccountChangePasswordRequest request, string accountId)
        {
            Account account = await GetByIdAsync(accountId);
            if (!account.AccountPassword.Equals((request.AccountPassword + account.AccountSaft).HashPassword()))
            {
                return false;
            }
            account.AccountPassword = (request.AccountNewPassword + account.AccountSaft).HashPassword();
            RepositoryWrapper.Accounts.Update(account);
            await RepositoryWrapper.SaveChangesAsync();
            return true;
        }
        public async Task SendPasswordRecoveryLinkAsync(AccountResetPasswordRequest request)
        {
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountEmail.Equals(request.AccountEmail)).FirstOrDefaultAsync();
            RecoveryTokens recoveryTokens = new(account.AccountId, Random(RNGCryptoServiceProvider, 100000, 999999));
            MailMessage mailMessage = new()
            {
                Subject = "Xác nhận cấp lại mật khẩu",
                //Body = "We heard that you lost your SSO password. Sorry about that!"
                //+ $"<br/> But don’t worry! You can use the following link to reset your password: <a href='{Configuration["PasswordRecovery:PasswordResetLink"]}/{recoveryTokens.RecoveryToken}'>Reset Your Password</a>"
                //+ $"<br/> If you don’t use this link within {Configuration["PasswordRecovery:ExpiredTime"]} hours, it will expire."
                //+ "<br/> Thanks, <br/>The SSO Team",
                Body = $"<table border='0' cellpadding='0' cellspacing='0' width='100%'> <tr> <td align='center'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'> <tr> <td align='left' bgcolor='#6600ff'> <h1 style='margin: 0; font-size: 32px; font-weight: 700; letter-spacing: -1px; line-height: 48px; color: #ffffff;padding: 20px;'>Hệ thống thông tin nguồn</h1> </td> </tr> </table> </td> </tr> <tr> <td align='center'> <table border='0' cellpadding='0' cellspacing='0' width='100%' style='max-width: 600px;'> <tr> <td align='left' bgcolor='#dddddd'> <p style='margin: 0; padding: 45px 20px 35px 20px;font-size: 20px; line-height: 24px;'><span style='font-weight: bold; color: #0000ff;'>Xin chào/ Hello:</span> {account.AccountUserName}</p> </td> </tr> <tr> <td align='left' bgcolor='#dddddd'> <p style='margin: 0;padding: 0px 20px; font-size: 16px; line-height: 24px;'>Đây là email giúp bạn tạo mật khẩu mới cho tài khoản người dùng trên <span style='color: #0000ff'>Hệ thống thông tin nguồn</span></p> <p style='margin: 0;padding: 0px 20px;font-size: 16px; line-height: 24px;'>This is email help you create new password to user account on the source information system.</p> <p style='margin: 20px 20px 0px 20px; font-size: 16px; line-height: 24px;'>Vui lòng <a href='{Configuration["PasswordRecovery:PasswordResetLink"]}/{recoveryTokens.RecoveryToken}'>Click vào đây</a> để tạo mật khẩu mới.</p> <p style='margin: 0px 20px; font-size: 16px; line-height: 24px;'>Please <a href='{Configuration["PasswordRecovery:PasswordResetLink"]}/{recoveryTokens.RecoveryToken}'>Click here</a> to create a new password.</p> <p style='margin: 20px 20px 0px 20px; font-size: 16px; line-height: 24px;'>Lưu ý: Đây là email tự động của hệ thống. Vui lòng không trả lời email này.</p> <p style='margin: 0px 20px 20px 20px; font-size: 16px; line-height: 24px;'>Note: This is the system's automated email. Please do not reply to this email.</p> </td> </tr> </table> </td> </tr> </table>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(request.AccountEmail);
            ObserverPattern.Instance.Emit("SendMailSMTP", mailMessage);
            await RedisRepository.SetRecordAsync(recoveryTokens.RecoveryToken.ToString(), recoveryTokens, TimeSpan.FromHours(int.Parse(Configuration["PasswordRecovery:ExpiredTime"])));
        }
        public async Task<RecoveryTokens> CheckRecoveryTokenAsync(int? recoveryToken)
        {
            RecoveryTokens recoveryTokens = await RedisRepository.GetRecordAsync<RecoveryTokens>(recoveryToken.ToString());
            if (recoveryTokens is not null && recoveryTokens.RecoveryToken.Equals(recoveryToken))
            {
                return recoveryTokens;
            }
            return null;
        }
        public async Task<(Account account, string message)> RegisterAsync(AccountRegisterRequest request)
        {
            Account account = Mapper.Map<Account>(request);
            if (request.AccountAvatar is not null)
            {
                #region Save Device Image to AmazonS3

                (string ResourceUrl, string ErrorMessageSaveFileAmazonS3) = await AmazonS3Utility.SaveFileAmazonS3Async(request.AccountAvatar, Configuration["AmazonS3:BucketName"], AccountAvatar + "/" + DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(request.AccountAvatar.FileName.Trim(), @"[^a-zA-Z0-9-_.]", ""), S3CannedACL.PublicRead);
                if (!string.IsNullOrEmpty(ErrorMessageSaveFileAmazonS3))
                {
                    return (null, ErrorMessageSaveFileAmazonS3);
                }
                account.AccountAvatar = ResourceUrl;
                #endregion
            }
            if(request.AccountTwoFactorAuth is null)
            {
                account.AccountTwoFactorAuth = AccountTwoFactorAuth.Off;
            }
            account.AccountType = AccountType.Normal;
            account.AccountSaft = 6.RandomString();
            account.AccountPassword = (account.AccountPassword + account.AccountSaft).HashPassword();
            RepositoryWrapper.Accounts.Add(account);
            await RepositoryWrapper.SaveChangesAsync();
            return (account, "StoreAccountSuccess");
        }
        public async Task<(Account account, string message)> StoreAsync(AccountStoreRequest request)
        {
            Account account = Mapper.Map<Account>(request);
            if (request.AccountAvatar is not null)
            {
                #region Save Device Image to AmazonS3

                (string ResourceUrl, string ErrorMessageSaveFileAmazonS3) = await AmazonS3Utility.SaveFileAmazonS3Async(request.AccountAvatar, Configuration["AmazonS3:BucketName"], AccountAvatar + "/" + DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(request.AccountAvatar.FileName.Trim(), @"[^a-zA-Z0-9-_.]", ""), S3CannedACL.PublicRead);
                if (!string.IsNullOrEmpty(ErrorMessageSaveFileAmazonS3))
                {
                    return (null, ErrorMessageSaveFileAmazonS3);
                }
                account.AccountAvatar = ResourceUrl;
                #endregion
            }
            account.AccountType = AccountType.SSO;
            account.AccountSaft = 6.RandomString();
            account.AccountPassword = (request.AccountPassword + account.AccountSaft).HashPassword();
            RepositoryWrapper.Accounts.Add(account);
            List<Permission> permissions = await RepositoryWrapper.Permissions.Find(x => request.PermissionCodes.Any(y => y.Equals(x.PermissionCode))).ToListAsync();
            List<AccountPermission> AccountPermissions = new();
            foreach (Permission permission in permissions)
            {
                AccountPermission accountPermision = new(permission.PermissionCode, account.AccountId);
                AccountPermissions.Add(accountPermision);
            }
            RepositoryWrapper.AccountPermissions.AddRange(AccountPermissions);
            await RepositoryWrapper.SaveChangesAsync();
            return (account, "StoreAccountSuccess");
        }
        public async Task<Account> GetByIdAsync(string accountId)
        {
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountId.Equals(accountId)).Include(x => x.AccountPermissions).FirstOrDefaultAsync();
            return account;
        }
        public async Task<Account> GetProfileAsync(string accountId)
        {
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountId.Equals(accountId)).Include(x => x.AccountPermissions).FirstOrDefaultAsync();
            return account;
        }
        private async Task<AccountOTPResponse> GetOTPAsync(Account account)
        {
            string oTPCode = 6.RandomNumberString();
            account.AccountOTPCode = oTPCode;
            AccountOTPResponse oTPResponse = new(account.AccountId, oTPCode);
            await RedisRepository.SetRecordAsync(account.AccountId, account, TimeSpan.FromSeconds(int.Parse(Configuration["OTPLoginSettings:ExpiredTime"])));
            return oTPResponse;
        }
        public async Task<AccountOTPResponse> GetNewOTPCodeAsync(Account account)
        {
            if (await RedisRepository.GetRecordAsync<AccountLoginRequest>(account.AccountPhone) is not null)
            {
                Account currentAccount = await RedisRepository.GetRecordAsync<Account>(account.AccountId);
                string newOTPCode = "";
                if (currentAccount is not null)
                {
                    do
                    {
                        newOTPCode = 6.RandomNumberString();
                    } while (currentAccount.AccountOTPCode.Equals(newOTPCode));
                    currentAccount.AccountOTPCode = newOTPCode;
                    await RedisRepository.SetRecordAsync(account.AccountId, currentAccount, TimeSpan.FromSeconds(int.Parse(Configuration["OTPLoginSettings:ExpiredTime"])));
                    AccountOTPResponse accountOTPResponse = new(account.AccountId, newOTPCode);
                    return accountOTPResponse;
                }
            }
            return null;
        }
        public async Task<(Account, string)> UpdateProfileAsync(AccountUpdateProfileRequest request, string accountId)
        {
            if (!await IsPhoneDoesNotExistValidationAsync(request.AccountPhone, accountId))
            {
                return (null, "AccountPhoneAlreadyExists");
            }
            if(!await IsEmailDoesNotExistValidationAsync(request.AccountEmail, accountId))
            {
                return (null, "AccountEmailAlreadyExists");
            }
            Account account = await GetByIdAsync(accountId);
            Mapper.Map(request, account);
            #region Save Device Image to AmazonS3
            if (request.AccountAvatar is not null)
            {
                #region Save Account Avatar to AmazonS3
                (string ResourceUrl, string ErrorMessageSaveFileAmazonS3) = await AmazonS3Utility.SaveFileAmazonS3Async(request.AccountAvatar, Configuration["AmazonS3:BucketName"], AccountAvatar + "/" + DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(request.AccountAvatar.FileName.Trim(), @"[^a-zA-Z0-9-_.]", ""), S3CannedACL.PublicRead);
                if (!string.IsNullOrEmpty(ErrorMessageSaveFileAmazonS3))
                {
                    return (null, ErrorMessageSaveFileAmazonS3);
                }
                account.AccountAvatar = ResourceUrl;
                #endregion
            }
            #endregion
            RepositoryWrapper.Accounts.Update(account);
            await RepositoryWrapper.SaveChangesAsync();
            return (account, "UpdateProfileAccountSuccess");
        }
        public async Task<(Account, string)> UpdateAsync(AccountUpdateRequest request, string accountUpdateId, string accountId)
        {
            Account account = await GetByIdAsync(accountUpdateId);
            List<string> permissionCodes = await RepositoryWrapper.AccountPermissions.Find(x => x.AccountId.Equals(accountId)).Select(x => x.PermissionCode).ToListAsync();
            AccountType accountType = (AccountType)account.AccountType;
            if (accountType == AccountType.SSO && !permissionCodes.Any(y => y.Equals("ACCOUNT_SSO_UPDATE")))
            {
                return (null, "DoNotPermissionWithAccountId");
            }
            if (accountType == AccountType.Normal && !permissionCodes.Any(y => y.Equals("ACCOUNT_UPDATE")))
            {
                return (null, "DoNotPermissionWithAccountId");
            }
            AccountStatus currentStatus = (AccountStatus)account.AccountStatus;
            Mapper.Map(request, account);
            if(request.AccountPassword is not null)
            {
                account.AccountPassword = (request.AccountPassword + account.AccountSaft).HashPassword();
            }
            //if (account.AccountStatus != currentStatus)
            //{
            //    List<string> accountIds = new();
            //    accountIds.Add(account.AccountId);
            //    if (account.AccountStatus == AccountStatus.Active)
            //    {
            //        await SendNotifyToWebHook(accountIds, ActionType.ActiveAccount);
            //    }
            //    await SendNotifyToWebHook(accountIds, ActionType.DeactiveAccount);
            //}
            if (request.AccountAvatar is not null)
            {
                #region Save Account Avatar to AmazonS3
                (string ResourceUrl, string ErrorMessageSaveFileAmazonS3) = await AmazonS3Utility.SaveFileAmazonS3Async(request.AccountAvatar, Configuration["AmazonS3:BucketName"], AccountAvatar + "/" + DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(request.AccountAvatar.FileName.Trim(), @"[^a-zA-Z0-9-_.]", ""), S3CannedACL.PublicRead);
                if (!string.IsNullOrEmpty(ErrorMessageSaveFileAmazonS3))
                {
                    return (null, ErrorMessageSaveFileAmazonS3);
                }
                account.AccountAvatar = ResourceUrl;
                #endregion
            }
            RepositoryWrapper.Accounts.Update(account);
            if (account.AccountType == AccountType.SSO)
            {
                if (request.PermissionCodes is not null && request.PermissionCodes.Count != 0)
                {
                    List<AccountPermission> accountPermissions = await RepositoryWrapper.AccountPermissions.Find(x => x.AccountId.Equals(accountUpdateId)).ToListAsync();
                    RepositoryWrapper.AccountPermissions.RemoveRange(accountPermissions);
                    List<AccountPermission> newAccountPermissions = new();
                    List<Permission> permissions = await RepositoryWrapper.Permissions.Find(x => request.PermissionCodes.Any(y => y.Equals(x.PermissionCode))).ToListAsync();
                    foreach (Permission permission in permissions)
                    {
                        AccountPermission accountPermission = new(permission.PermissionCode, accountUpdateId);
                        newAccountPermissions.Add(accountPermission);
                    }
                    RepositoryWrapper.AccountPermissions.AddRange(newAccountPermissions);
                }
            }
            await RepositoryWrapper.SaveChangesAsync();
            return (account, "UpdateAccountSuccess");
        }
        public async Task<bool> DeleteManyAsync(AccountDeleteRequest request, string accountId)
        {
            List<string> accountPermissions = await RepositoryWrapper.AccountPermissions.Find(x => x.AccountId.Equals(accountId)).Select(x => x.PermissionCode).ToListAsync();
            List<Account> accounts = await RepositoryWrapper.Accounts.Find(x => request.AccountIds.Any(y => y.Equals(x.AccountId))).ToListAsync();
            AccountType accountType = (AccountType)accounts.Select(x => x.AccountType).FirstOrDefault();
            if (accountType == AccountType.SSO && !accountPermissions.Any(y => y.Equals("ACCOUNT_SSO_DELETE")))
            {
                return false;
            }
            if (accountType == AccountType.Normal && !accountPermissions.Any(y => y.Equals("ACCOUNT_DELETE")))
            {
                return false;
            }
            RepositoryWrapper.Accounts.RemoveRange(accounts);
            await RepositoryWrapper.SaveChangesAsync();
            List<string> accountIds = accounts.Select(x => x.AccountId).ToList();
            //await SendNotifyToWebHook(accountIds, ActionType.DeleteAccount);
            return true;
        }
        public async Task<PaginationResponse<Account>> GetAllAsync(GetAllAccountSSORequest request)
        {
            IQueryable<Account> accounts = RepositoryWrapper.Accounts.Find
            (x =>
                (
                    request.AccountType == null || x.AccountType == request.AccountType
                )
                &&
                (
                    string.IsNullOrEmpty(request.SearchContent) || (
                                                                        x.AccountFullName.ToLower().Contains(request.SearchContent.ToLower()) ||
                                                                        x.AccountEmail.ToLower().Contains(request.SearchContent.ToLower()) ||
                                                                        x.AccountPhone.Contains(request.SearchContent) ||
                                                                        x.AccountUserName.Contains(request.SearchContent)
                                                                   )
                ) &&
                (
                    request.AccountStatus == null || x.AccountStatus == request.AccountStatus
                )
            )
            .Include(x => x.AccountPermissions)
            .OrderByDescending(x => x.AccountCreateAt);
            accounts = SortUtility<Account>.ApplySort(accounts, request.OrderByQuery);
            PaginationUtility<Account> result = await PaginationUtility<Account>.ToPagedListAsync(accounts, request.PageNumber, request.PageSize);
            PaginationResponse<Account> paginationResponse = PaginationResponse<Account>.PaginationInfo(result, result.PageInfo);
            return paginationResponse;
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        #region Validation
        public async Task<bool> IsPasswordCorrectValidationAsync(string accountUserName, string accountPassword)
        {
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountUserName.Equals(accountUserName) || x.AccountPhone.Equals(accountUserName)).FirstOrDefaultAsync();
            if (account is null)
            {
                return false;
            }
            return (accountPassword + account.AccountSaft).HashPassword().Equals(account.AccountPassword);
        }
        public async Task<bool> IsAccountUserNameExistsValidationAsync(string accountUserName)
        {
            Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountUserName.Equals(accountUserName) || x.AccountPhone.Equals(accountUserName)).FirstOrDefaultAsync();
            return account is not null;
        }
        public async Task<bool> IsUserNameDoesNotExistsValidationAsync(string userName, string accountId = null)
        {
            if (accountId is null)
            {
                Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountUserName.Equals(userName)).FirstOrDefaultAsync();
                return account is null;
            }
            else
            {
                Account user = await RepositoryWrapper.Accounts.Find(x => x.AccountUserName.Equals(userName) &&  !x.AccountId.Equals(accountId)).FirstOrDefaultAsync();
                return user is null;
            }

        }
        public bool IsValidUrlValidation(string redirectUrl)
        {
            Uri outUri;
            return Uri.TryCreate(redirectUrl, UriKind.Absolute, out outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }

        public async Task<bool> IsEmailDoesNotExistValidationAsync(string accountEmail, string accountId = null)
        {
            if (accountId is null)
            {
                Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountEmail.Equals(accountEmail)).FirstOrDefaultAsync();
                return account is null;
            }
            else
            {
                Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountEmail.Equals(accountEmail) && !x.AccountId.Equals(accountId)).FirstOrDefaultAsync();
                return account is null;
            }
        }

        public async Task<bool> IsPhoneDoesNotExistValidationAsync(string accountPhone, string accountId = null)
        {
            if (accountId is null)
            {
                Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountPhone.Equals(accountPhone)).FirstOrDefaultAsync();
                return account is null;
            }
            else
            {
                Account account = await RepositoryWrapper.Accounts.Find(x => x.AccountPhone.Equals(accountPhone) && !x.AccountId.Equals(accountId)).FirstOrDefaultAsync();
                return account is null;
            }
        }
        public async Task<bool> IsCorrectOTPCodeValidationAsync(string accountId, string oTPCode)
        {
            Account accountCurrent = await RedisRepository.GetRecordAsync<Account>(accountId);
            if (accountCurrent is null)
            {
                return false;
            }
            if (accountCurrent.AccountOTPCode is not null && accountCurrent.AccountOTPCode.Equals(oTPCode))
            {
                await RedisRepository.DeleteRecordAsync(accountId);
                return true;
            }
            int overTime = 0;
            if (await RedisRepository.GetRecordAsync<int>(accountCurrent.AccountUserName) == 0)
            {
                await RedisRepository.SetRecordAsync(accountCurrent.AccountUserName, overTime + 1, TimeSpan.FromMinutes(int.Parse(Configuration["OTPLoginSettings:ExpiredLockUser"])));
                return false;
            }
            overTime = await RedisRepository.GetRecordAsync<int>(accountCurrent.AccountUserName) + 1;

            await RedisRepository.SetRecordAsync(accountCurrent.AccountUserName, overTime, TimeSpan.FromMinutes(int.Parse(Configuration["OTPLoginSettings:ExpiredLockUser"])));
            if (overTime > int.Parse(Configuration["OTPLoginSettings:ExpiredLockUser"]))
            {
                return true;
            }
            return false;
        }
        public async Task<bool> IsIdentityCardDoesNotExistsValidationAsync(string accountIdentityCard, string accountId = null)
        {
            if (accountId is null)
            {
                return await RepositoryWrapper.Accounts.Find(x => x.AccountIdentityCard.Equals(accountIdentityCard)).FirstOrDefaultAsync() is null;
            }
            else
            {
                return await RepositoryWrapper.Accounts.Find(x => x.AccountIdentityCard.Equals(accountIdentityCard) && x.AccountId.Equals(accountId)).FirstOrDefaultAsync() is null;
            }
        }

        public async Task<bool> IsApplicationSecretValidValidationAsync(string applicationId, string applicationSecret)
        {
            return await RepositoryWrapper.Applications.Find(x => x.ApplicationId.Equals(applicationId) && x.ApplicationSecret.Equals(applicationSecret)).FirstOrDefaultAsync() is not null;
        }
        public async Task<bool> IsPermissionCodesExistsValidationAsync(List<string> permissionCodes)
        {
            List<Permission> permissions = await RepositoryWrapper.Permissions.Find(x => permissionCodes.Any(y => y.Equals(x.PermissionCode))).ToListAsync();
            return permissionCodes.Count == permissions.Count;
        }

        public async Task<bool> IsAccountIdsExistsValidationAsync(List<string> accountIds)
        {
            List<Account> accounts = await RepositoryWrapper.Accounts.Find(x => accountIds.Any(y => y.Equals(x.AccountId))).ToListAsync();
            return accountIds.Count == accounts.Count;
        }
        public async Task<bool> IsCorrectTryNumberInputOTPAsync(string accountId, string oTPCode)
        {
            Account accountCurrent = await RedisRepository.GetRecordAsync<Account>(accountId);
            if (accountCurrent is not null)
            {
                if (await RedisRepository.GetRecordAsync<int>(accountCurrent.AccountUserName) < int.Parse(Configuration["OTPLoginSettings:OverTime"]))
                {
                    return true;
                }
                Account account = await GetByIdAsync(accountId);
                account.AccountStatus = AccountStatus.Locked;
                RepositoryWrapper.Accounts.Update(account);
                await RepositoryWrapper.SaveChangesAsync();
                await RedisRepository.DeleteRecordAsync(accountCurrent.AccountUserName);
                await RedisRepository.DeleteRecordAsync(accountId);
                return false;
            }
            return true;
        }
        public async Task<bool> IsAccountUserNameActiveValidationAsync(string accountUserName)
        {
            return await RepositoryWrapper.Accounts.Find(x => (x.AccountUserName.Equals(accountUserName) || x.AccountPhone.Equals(accountUserName)) && x.AccountStatus == AccountStatus.Active).FirstOrDefaultAsync() is not null;
        }
        public async Task<bool> IAccountPhoneActiveValidationAsync(string accountPhone)
        {
            return await RepositoryWrapper.Accounts.Find(x => x.AccountPhone.Equals(accountPhone) && x.AccountStatus == AccountStatus.Active).FirstOrDefaultAsync() is not null;
        }

        public async Task<bool> IsEmailExistValidationAsync(string accountEmail)
        {
            return await RepositoryWrapper.Accounts.Find(x => x.AccountEmail.Equals(accountEmail)).FirstOrDefaultAsync() is not null;
        }
        public async Task<bool> IsPhoneExistValidationAsync(string accountPhone, string applicationId = null)
        {
            return await RepositoryWrapper.Accounts.Find
            (x =>
                x.AccountPhone.Equals(accountPhone)
                &&
                (string.IsNullOrEmpty(applicationId)) || x.ApplicationId.Equals(applicationId)
            ).FirstOrDefaultAsync() is not null;
        }
        public async Task<bool> IsAccountPhoneDoesNotExistValidationAsync(string accountPhone)
        {
            Account account = await RepositoryWrapper.Accounts.Find
                (x =>
                    x.AccountPhone.Equals(accountPhone)
                ).FirstOrDefaultAsync();
            return account is not null;
        }
        public bool IsValidContentType(IFormFile accountAvatar, List<string> contentTypes)
        {
            foreach (string contentType in contentTypes)
            {
                if (accountAvatar.ContentType.StartsWith(contentType))
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<bool> IsAccountStatusSameAvailable(string accountId, AccountStatus? accountStatus)
        {
            return await RepositoryWrapper.Accounts.Find(x => x.AccountId.Equals(accountId) && x.AccountStatus == accountStatus).FirstOrDefaultAsync() is null;
        }
        public async Task<bool> IsAccountTwoFactorAuthSameAvailable(string accountId, AccountTwoFactorAuth? accountTwoFactorAuth)
        {
            return await RepositoryWrapper.Accounts.Find(x => x.AccountId.Equals(accountId) && x.AccountTwoFactorAuth == accountTwoFactorAuth).FirstOrDefaultAsync() is null;
        }
        #endregion
        private int Random(RNGCryptoServiceProvider rNGCryptoServiceProvider, int min, int max)
        {
            byte[] randomNumber = new byte[sizeof(UInt32)];
            rNGCryptoServiceProvider.GetBytes(randomNumber);
            double value = BitConverter.ToUInt32(randomNumber, 0) / (double)UInt32.MaxValue;
            return min + (int)((max - min) * value); // (int)(value % max + min)
        }
        private async Task SendNotifyToWebHook(List<string> accountIds, ActionType actionType)
        {
            WebHookResponse webHookResponse = new(accountIds, actionType);
            List<Application> applications = await RepositoryWrapper.Applications.FindAll().ToListAsync();
            string sendContent;
            try
            {
                foreach (Application application in applications)
                {
                    sendContent = JsonConvert.SerializeObject(webHookResponse);
                    HttpMethod.Post.SendRequestWithStringContent(application.ApplicationWebHook, sendContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendRequestToWebHook: " + ex.GetBaseException().ToString());
                return;
            }
        }
        public async Task<bool> IsAccountIdsSameAsType(List<string> accountIds)
        {
            List<Account> accounts = await RepositoryWrapper.Accounts.Find(x => accountIds.Any(y => y.Equals(x.AccountId))).ToListAsync();
            return accounts.GroupBy(x => x.AccountType).ToList().Count == 1;
        }

        public bool VerifyCaptchaAsync(string token)
        {
            (string responseData, int? statusCode) = HttpMethod.Post.SendRequestWithStringContent($"{Configuration["GooglereCAPTCHA:VerifyLink"]}?secret={Configuration["GooglereCAPTCHA:SecretKey"]}&response={token}");
            if(statusCode != 200)
            {
                return false;
            }
            ReCaptchaResponse reCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(responseData);
            Console.WriteLine("Score: " + reCaptchaResponse.Score);
            return reCaptchaResponse.Success && reCaptchaResponse.Score >= float.Parse($"{Configuration["GooglereCAPTCHA:Score"]}");
        }
    }
}
