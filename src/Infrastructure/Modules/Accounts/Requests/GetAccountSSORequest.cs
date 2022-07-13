using Core.Ultilities;
using Infrastructure.Modules.Accounts.Entities;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class GetAllAccountSSORequest : PaginationRequest
    {
        public AccountStatus? AccountStatus { get; set; }
        public AccountType? AccountType { get; set; }
    }
}
