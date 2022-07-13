using Newtonsoft.Json;
using Infrastructure.Modules.Applications.Entities;
using System;
using System.Collections.Generic;

namespace Infrastructure.Modules.Accounts.Responses
{
    public class AccountLoginResponse
    {
        public AccountLoginResponse(string accessToken, int expiredAt, /*string refreshToken,*/ string redirectUrl)
        {
            access_token = accessToken;
            expired_at = expiredAt;
            //RefreshToken = refreshToken;
            RedirectUrl = redirectUrl;
        }

        public string access_token { get; set; }
        public int expired_at { get; set; }
        //public string RefreshToken { get; set; }
        [JsonIgnore]
        public string RedirectUrl { get; set; }
    }   
}
