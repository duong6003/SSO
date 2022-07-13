namespace Infrastructure.Modules.Applications.Requests
{
    public class ApplicationGetTokenRequest
    {
        public string ApplicationId { get; set; }
        public string ApplicationSecret { get; set; }
    }
}
