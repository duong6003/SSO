using Microsoft.AspNetCore.Http;

namespace Infrastructure.Modules.Applications.Requests
{
    public class ApplicationRegisterRequest
    {
        public string ApplicationName { get; set; }
        public IFormFile ApplicationIcon { get; set; }
        public string ApplicationRedirectUrl { get; set; }
        public string  ApplicationWebHook { get; set; }
    }
}
