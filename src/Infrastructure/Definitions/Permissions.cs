using System.ComponentModel;

namespace Infrastructure.Definitions;

public class Permissions
{
    [DisplayName("Users")]
    public static class Users
    {
        public const string View = "Per.Users.View";
        public const string Create = "Per.Users.Create";
        public const string Edit = "Per.Users.Edit";
        public const string Delete = "Per.Users.Delete";
        public const string Export = "Per.Users.Export";
    }
}
