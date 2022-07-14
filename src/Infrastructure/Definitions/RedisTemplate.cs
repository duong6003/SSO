using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Infrastructure.Definitions
{
    public static class RedisTemplate
    {
        private static string SignIn = "SignIn-[PhoneNumber]";
        private static string SignUp = "SignUp-[PhoneNumber]";
        private static string ChangePassword = "ChangePassword-[EmailAddress]";

        public static string GetSignInKeyByPhone(this string phoneNumber)
        {
            return SignIn.Replace("[PhoneNumber]", phoneNumber);
        }
        public static string GetSignUpKeyByPhone(this string phoneNumber)
        {
            return SignUp.Replace("[PhoneNumber]", phoneNumber);
        }
        public static string GetChangePasswordKeyByEmail(this string emailAddress)
        {
            return ChangePassword.Replace("[EmailAddress]", emailAddress);
        }
    }
}
