namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountResetNewPasswordRequest 
    {
        public string? NewPassword { get; set; }
        public string? RecoveryToken { get; set; }
    }
}
