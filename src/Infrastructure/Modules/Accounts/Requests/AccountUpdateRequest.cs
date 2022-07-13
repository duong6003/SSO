using Microsoft.AspNetCore.Http;
using Infrastructure.Modules.Accounts.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountUpdateRequest 
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string? IdentityCard { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public AccountStatus? AccountStatus { get; set; }
        public TwoFactorAuth? TwoFactorAuth { get; set; }
        public List<string>? PermissionCodes { get; set; }
    }
}
