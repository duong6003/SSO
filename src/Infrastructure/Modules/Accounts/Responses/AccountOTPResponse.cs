namespace Infrastructure.Modules.Accounts.Responses
{
    public class AccountOTPResponse
    {
        public string AccountId { get; set; }
        public string OTPCode { get; set; }
        public AccountOTPResponse(string accountId, string oTPCode)
        {
            AccountId = accountId;
            OTPCode = oTPCode;
        }
    }
}
