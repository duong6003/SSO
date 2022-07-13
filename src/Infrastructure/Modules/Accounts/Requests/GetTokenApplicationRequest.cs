namespace Infrastructure.Modules.Accounts.Requests
{
    public class GetTokenApplicationRequest
    {
        public string? Client_id { get; set; }
        public string? Client_secret { get; set; }
        public string? Code { get; set; }
    }
}
