namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountLoginRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ApplicationId { get; set; }
        public string? SuccessUrl { get; set; }
        public string? ReCaptcha { get; set; }
    }
}
