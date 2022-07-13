using System.ComponentModel;

namespace Infrastructure.Definitions;

public class Messages
{
    [DisplayName("Middlewares")]
    public static class Middlewares
    {
        public const string IPAddressForbidden = "Mes.Middlewares.IPAddress.Forbidden";
    }

    [DisplayName("Accounts")]
    public static class Accounts
    {
        public const string NotFound = "Messages.Accounts.NotFound";
        public const string InActive = "Messages.Accounts.InActive";
        public const string LockedEnteringWrongPasswordManyTime = "Messages.Accounts.LockedEnteringWrongPasswordManyTime";
        public const string IdNotFound = "Messages.Accounts.Id.NotFound";
        public const string IdIsRequired = "Messages.Accounts.Id.IsRequired";
        public const string IdsInValid = "Messages.Accounts.Ids.InValid";
        public const string UserNameIsRequired = "Messages.Accounts.UserName.IsRequired";
        public const string UserNameMussLessThan255Character = "Messages.Accounts.UserName.MussLessThan255Character";
        public const string IdentityCardMustNumericCharacter = "Messages.Accounts.IdentityCard.MustNumericCharacter";
        public const string IdentityCardMustLessThan13Character = "Messages.Accounts.IdentityCard.MustLessThan13Character";
        public const string EmailIsRequired = "Messages.Accounts.Email.IsRequired";
        public const string EmailMustBeEmail = "Messages.Accounts.Email.MustBeEmail";
        public const string EmailAlreadyExists = "Messages.Accounts.Email.AlreadyExists";
        public const string FullNameIsRequired = "Messages.Accounts.FullName.IsRequired";
        public const string FullNameMussLessThan255Character = "Messages.Accounts.FullName.MussLessThan255Character";
        public const string PasswordIsRequired = "Messages.Accounts.Password.IsRequired";
        public const string TwoFactorAuthInValid = "Messages.Accounts.TwoFactorAuth.InValid";
        public const string GetProfileSuccess = "Messages.Accounts.GetProfile.Success";
        public const string GetSuccess = "Messages.Accounts.Get.Success";
    }
    [DisplayName("Roles")]
    [Description("Roles Messages")]
    public static class Roles
    {
        public const string IdNotFound = "Mes.Roles.Id.NotFound";
        public const string GetSuccess = "Messages.Roles.Get.Success";
    }

    [DisplayName("Permissions")]
    [Description("Permissions Messages")]
    public static class Permissions
    {
        public const string GetSuccess = "Messages.Permissions.Get.Success";
    }
}