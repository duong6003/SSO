namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountChangePasswordRequest
    {
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
    }
}
