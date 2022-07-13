namespace Infrastructure.Definitions
{
    public static class RedisTemplate
    {
        private static string SignIn = "SignIn-[PhoneNumber]";
        private static string SignUp = "SignUp-[PhoneNumber]";
        private static string ChangePhoneNumber = "ChangePhoneNumber-[PhoneNumber]";
        private static string ForgotPassword = "ForgotPassword-[PhoneNumber]";

        public static string GetSignInKeyByPhone(this string phoneNumber)
        {
            return SignIn.Replace("[PhoneNumber]", phoneNumber);
        }
        public static string GetSignUpKeyByPhone(this string phoneNumber)
        {
            return SignUp.Replace("[PhoneNumber]", phoneNumber);
        }
        public static string GetChangePhoneNumberKey(this string phoneNumber)
        {
            return ChangePhoneNumber.Replace("[PhoneNumber]", phoneNumber);
        }
        public static string GetForgotPasswordKey(this string phoneNumber)
        {
            return ForgotPassword.Replace("[PhoneNumber]", phoneNumber);
        }
    }
}
