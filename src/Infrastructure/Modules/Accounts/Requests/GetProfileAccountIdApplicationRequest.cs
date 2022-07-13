namespace Infrastructure.Modules.Accounts.Requests
{
    public class GetProfileAccountIdApplicationRequest 
    {
        public string? ApplicationId { get; set; }
        public string? ApplicationSecret { get; set; }
        public string? AccountId { get; set; }
    }
}
