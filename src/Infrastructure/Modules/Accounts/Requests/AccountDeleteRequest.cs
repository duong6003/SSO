using System.Collections.Generic;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountDeleteRequest
    {
        public List<string>? AccountIds { get; set; }
    }
}
