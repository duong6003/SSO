using Amazon.S3;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Infrastructure.App.Databases;
using Infrastructure.App.DesignPatterns.Reponsitories;
using Infrastructure.App.Utilities;
using Infrastructure.Modules.Accounts.Entities;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Entities;
using Infrastructure.Modules.Applications.Requests;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace Infrastructure.Modules.Applications.Services
{
    public interface IApplicationService
    {
        Task<(Application, string message)> RegisterAsync(ApplicationRegisterRequest request, string userId);
        Task<Application> GetByIdAsync(string applicationId);
        Task DeleteAsync(Application application);
        Task<object> GetNewSecretKeyAsync(string applicationId);
        Task<Application> UpdateStatusAsync(ApplicationUpdateStatusRequest request, string applicationId);
        Task<(Application application, string message)> UpdateAsync(ApplicationUpdateRequest request, string applicationId);
        Task<PaginationResponse<Account>> GetByApplicationIdAsync(GetAllAccountApplicationRequest request, string applicationId);
        Task<Application> GetDetailAsync(string applicationId);
        Task<List<Application>> AllowedAccessAsync(string accountId);
        Task<List<Application>> GetAll();

        #region Validation
        Task<bool> IsApplicationSecretValidValidationAsync(string applicationId ,string applicationSecret);
        Task<bool> IsApplicationIdValidValidationAsync(string applicationId);
        bool IsUrlValidValidation(string redirectUrl);
        Task<bool> IsApplicationStatusSameAvailable(string applicationId, ApplicationStatus? applicationStatus);
        Task<bool> IsApplicationActiveValidationAsync(string applicationId);
        #endregion
    }
    public class ApplicationService : IApplicationService
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        private readonly IMapper Mapper;
        private readonly IAccountService AccountService;
        private readonly IDatabase RedisRepository;
        private readonly IConfiguration Configuration;
        private readonly IAmazonS3Utility AmazonS3Utility;
        private readonly string FolderApplicationIcon = "ApplicationIcon";

        public ApplicationService(IRepositoryWrapperMariaDB repositoryWrapperMariaDB, IMapper mapper, IAccountService accountService, IRedisDatabaseProvider redisDatabaseProvider, IConfiguration configuration, IAmazonS3Utility amazonS3Utility)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
            Mapper = mapper;
            AccountService = accountService;
            RedisRepository = redisDatabaseProvider.GetDatabase();
            Configuration = configuration;
            AmazonS3Utility = amazonS3Utility;
        }
        public async Task<List<Application>> GetAll()
        {
            List<Application> applications = await RepositoryWrapperMariaDB.Applications.FindAll().ToListAsync();
            return applications;
        }

        public async Task<Application> ApplicationNameDoesNotExistsValidationAsync(string accountId, string applicationName, string applicationId = null)
        {
            if(accountId is null)
            {
                Application applications = await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.AccountId.Equals(accountId) && x.ApplicationName.Equals(applicationName)).FirstOrDefaultAsync();
                return applications;
            }
            else
            {
                Application applications = await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.AccountId.Equals(accountId) && x.ApplicationName.Equals(applicationName) && !x.ApplicationId.Equals(applicationId)).FirstOrDefaultAsync();
                return applications;
            }
        }
        public async Task<object> GetNewSecretKeyAsync(string applicationId)
        {
            Application application = await GetByIdAsync(applicationId);
            application.ApplicationSecret = GenerateRandomSecret();
            RepositoryWrapperMariaDB.Applications.Update(application);
            await RepositoryWrapperMariaDB.SaveChangesAsync();
            return new { applicationSecret = application.ApplicationSecret };
        }
        public async Task<Application> UpdateStatusAsync(ApplicationUpdateStatusRequest request, string applicationId)
        {
            Application application = await GetByIdAsync(applicationId);
            application.ApplicationStatus = (ApplicationStatus)request.ApplicationStatus;
            RepositoryWrapperMariaDB.Applications.Update(application);
            await RepositoryWrapperMariaDB.SaveChangesAsync();
            return application;
        }
        public async Task<PaginationResponse<Account>> GetByApplicationIdAsync(GetAllAccountApplicationRequest request, string applicationId)
        {
            IQueryable<Account> accounts = RepositoryWrapperMariaDB.Accounts.FindByCondition
            (x =>
                (
                    x.ApplicationId.Equals(applicationId) || x.ApplicationId == null
                )
                &&
                (
                    string.IsNullOrEmpty(request.SearchContent) || (
                                                                        x.AccountFullName.ToLower().Contains(request.SearchContent.ToLower()) ||
                                                                        x.AccountEmail.ToLower().Contains(request.SearchContent.ToLower()) ||
                                                                        x.AccountPhone.Contains(request.SearchContent) ||
                                                                        x.AccountUserName.Contains(request.SearchContent) ||
                                                                        x.AccountIdentityCard.Contains(request.SearchContent)
                                                                   )
                ) &&
                (
                    request.AccountStatus == null || x.AccountStatus == request.AccountStatus
                ) &&
                (
                    x.AccountType == AccountType.Normal
                )
            ).OrderByDescending(x => x.AccountCreateAt);
            accounts = SortUtility<Account>.ApplySort(accounts, request.OrderByQuery);
            PaginationUtility<Account> result = await PaginationUtility<Account>.ToPagedListAsync(accounts, request.PageNumber, request.PageSize);
            PaginationResponse<Account> paginationResponse = PaginationResponse<Account>.PaginationInfo(result, result.PageInfo);
            return paginationResponse;
        }
        public async Task DeleteAsync(Application application)
        {
            RepositoryWrapperMariaDB.Applications.Remove(application);
            await RepositoryWrapperMariaDB.SaveChangesAsync();
        }
        
        public async Task<(Application, string message)> RegisterAsync(ApplicationRegisterRequest request, string accountId)
        {
            Application application = Mapper.Map<Application>(request);
            application.AccountId = accountId;
            application.ApplicationSecret = GenerateRandomSecret();
            if(request.ApplicationIcon is not null)
            {
                #region Save Application Icon to AmazonS3

                (string ResourceUrl, string ErrorMessageSaveFileAmazonS3) = await AmazonS3Utility.SaveFileAmazonS3Async(request.ApplicationIcon, Configuration["AmazonS3:BucketName"], FolderApplicationIcon + "/" + DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(request.ApplicationIcon.FileName.Trim(), @"[^a-zA-Z0-9-_.]", ""), S3CannedACL.PublicRead);
                if (!string.IsNullOrEmpty(ErrorMessageSaveFileAmazonS3))
                {
                    return (null, ErrorMessageSaveFileAmazonS3);
                }
                application.ApplicationIcon = ResourceUrl;
                #endregion
            }
            RepositoryWrapperMariaDB.Applications.Add(application);
            await RepositoryWrapperMariaDB.SaveChangesAsync();
            return (application, "RegisterApplicationSuccess");
        }
        public async Task<(Application application, string message)> UpdateAsync(ApplicationUpdateRequest request, string applicationId)
        {
            Application application = await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationId.Equals(applicationId)).FirstOrDefaultAsync();
            Mapper.Map(request, application);
            if (request.ApplicationIcon is not null)
            {
                #region Save Application Icon to AmazonS3
                (string ResourceUrl, string ErrorMessageSaveFileAmazonS3) = await AmazonS3Utility.SaveFileAmazonS3Async(request.ApplicationIcon, Configuration["AmazonS3:BucketName"], FolderApplicationIcon + "/" + DateTime.UtcNow.Ticks.ToString() + "_" + Regex.Replace(request.ApplicationIcon.FileName.Trim(), @"[^a-zA-Z0-9-_.]", ""), S3CannedACL.PublicRead);
                if (!string.IsNullOrEmpty(ErrorMessageSaveFileAmazonS3))
                {
                    return (null, ErrorMessageSaveFileAmazonS3);
                }
                application.ApplicationIcon = ResourceUrl;
                #endregion
            }
            RepositoryWrapperMariaDB.Applications.Update(application);
            await RepositoryWrapperMariaDB.SaveChangesAsync();
            return (application, "UpdateApplicationSuccess");
        }
        public async Task<Application> GetByIdAsync(string applicationId)
        {
            return await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationId.Equals(applicationId)).FirstOrDefaultAsync();
        }
        public async Task<Application> GetDetailAsync(string applicationId)
        {
            return await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationId.Equals(applicationId)).Include(x => x.Account).FirstOrDefaultAsync();
        }

        #region Validation
        public async Task<bool> IsApplicationSecretValidValidationAsync(string applicationId ,string applicationSecret)
        {
            Application app = await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationSecret.Equals(applicationSecret) && x.ApplicationId.Equals(applicationId)).FirstOrDefaultAsync();
            return app is not null;
        }

        public async Task<bool> IsApplicationIdValidValidationAsync(string applicationId)
        {
            Application app = await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationId.Equals(applicationId)).FirstOrDefaultAsync();
            return app is not null;
        }
        public bool IsUrlValidValidation(string redirectUrl)
        {
            Uri outUri;
            return Uri.TryCreate(redirectUrl, UriKind.Absolute, out outUri)
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
        public async Task<bool> IsApplicationStatusSameAvailable(string applicationId, ApplicationStatus? applicationStatus)
        {
            return await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationId.Equals(applicationId) && x.ApplicationStatus == applicationStatus).FirstOrDefaultAsync() is null;
        }
        #endregion

        private static string GenerateRandomSecret()
        {
            var  randomBytes = new byte[64];
            using RNGCryptoServiceProvider rNGCryptoServiceProvider = new();
            rNGCryptoServiceProvider.GetBytes(randomBytes);
            string secret = Base64UrlEncoder.Encode(randomBytes);
            return secret;
        }

        public async Task<List<Application>> AllowedAccessAsync(string accountId)
        {
            List<Application> applications = new();
            Account account = await AccountService.GetByIdAsync(accountId);
            if (account.AccountType == AccountType.Normal)
            {
                applications = await RepositoryWrapperMariaDB.Applications.FindByCondition(x => string.IsNullOrEmpty(account.ApplicationId) || x.ApplicationId.Equals(account.ApplicationId)).ToListAsync();
                foreach (Application application in applications)
                {
                    string code = await GenerateCodeAsync(accountId, application.ApplicationId);
                    if (application.ApplicationRedirectUrl.Contains("?"))
                    {
                        application.ApplicationRedirectUrl = $"{application.ApplicationRedirectUrl}&code={code}";
                    }
                    else
                    {
                        application.ApplicationRedirectUrl = $"{application.ApplicationRedirectUrl}?code={code}";
                    }    
                }
            }
            return applications;
        }

        private async Task<string> GenerateCodeAsync(string accountId, string applicationId)
        {
            string code = Guid.NewGuid().ToString();
            await RedisRepository.SetRecordAsync(code + applicationId, accountId, TimeSpan.FromHours(int.Parse(Configuration["CodeSettings:ExpiredTime"])));
            return code;

        }

        public async Task<bool> IsApplicationActiveValidationAsync(string applicationId)
        {
            return await RepositoryWrapperMariaDB.Applications.FindByCondition(x => x.ApplicationId.Equals(applicationId) && x.ApplicationStatus == ApplicationStatus.Active).FirstOrDefaultAsync() is not null;
        }
    }
}
