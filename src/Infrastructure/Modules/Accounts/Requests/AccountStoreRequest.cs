using Microsoft.AspNetCore.Http;
using Infrastructure.Modules.Accounts.Entities;
using System.Collections.Generic;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountStoreRequest
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public IFormFile? Avatar { get; set; }
        public string? IdentityCard { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public TwoFactorAuth? TwoFactorAuth { get; set; }
        public List<string>? PermissionCodes { get; set; }
    }
}
