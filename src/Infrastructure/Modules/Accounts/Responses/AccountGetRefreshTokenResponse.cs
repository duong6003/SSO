namespace Infrastructure.Modules.Accounts.Responses
{
    public class AccountGetRefreshTokenResponse
    {
        public AccountGetRefreshTokenResponse(string accessToken, int expiredAt, string refreshToken)
        {
            AccessToken = accessToken;
            ExpiredAt = expiredAt;
            RefreshToken = refreshToken;
        }

        public string AccessToken { get; set; }
        public int ExpiredAt { get; set; }
        public string RefreshToken { get; set; }
    }
}
