using Microsoft.AspNetCore.Http;
using Infrastructure.Modules.Accounts.Entities;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountUpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailAddress { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string? IdentityCard { get; set; }
        public IFormFile? Avatar { get; set; }
        public TwoFactorAuth? TwoFactorAuth { get; set; }
    }
}
