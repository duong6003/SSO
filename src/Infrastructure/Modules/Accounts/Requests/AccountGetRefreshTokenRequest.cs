namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountGetRefreshTokenRequest
    {
        public string? OldToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
