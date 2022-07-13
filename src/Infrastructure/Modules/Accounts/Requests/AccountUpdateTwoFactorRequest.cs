using Infrastructure.Modules.Accounts.Entities;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountUpdateTwoFactorRequest
    {
        public TwoFactorAuth? TwoFactorAuth { get; set; }
        public string? ApplicationId { get; set; }
        public string? ApplicationSecret { get; set; }
    }
}
