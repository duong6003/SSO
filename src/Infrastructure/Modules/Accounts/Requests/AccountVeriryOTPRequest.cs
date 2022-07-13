using Infrastructure.Modules.Accounts.Responses;

namespace Infrastructure.Modules.Accounts.Requests
{
    public class AccountVeriryOTPRequest : AccountOTPResponse
    {
        public AccountVeriryOTPRequest(string accountId, string oTPCode) : base(accountId, oTPCode)
        {
        }
    }
}
