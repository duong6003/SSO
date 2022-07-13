using Core.Ultilities;
using Infrastructure.Modules.Accounts.Entities;

namespace Infrastructure.Modules.Applications.Requests
{
    public class GetAllAccountApplicationRequest : PaginationRequest
    {
        public AccountStatus? AccountStatus { get; set; }
    }
}
