using Microsoft.AspNetCore.Http;
using Infrastructure.Modules.Accounts.Entities;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountRegisterRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        public string? IdentityCard { get; set; }
        public string? UserName { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? Password { get; set; }
        public TwoFactorAuth? TwoFactorAuth { get; set; }
        public string?  ApplicationId { get; set; }
        public string? ApplicationSecret { get; set; }
    }
}
