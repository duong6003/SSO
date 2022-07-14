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
    [DisplayName("Roles")]
    public static class Roles
    {
        public const string View = "Per.Roles.View";
        public const string Create = "Per.Roles.Create";
        public const string Edit = "Per.Roles.Edit";
        public const string Delete = "Per.Roles.Delete";
        public const string Export = "Per.Roles.Export";
    }
    [DisplayName("Products")]
    public static class Products
    {
        public const string View = "Per.Products.View";
        public const string Create = "Per.Products.Create";
        public const string Edit = "Per.Products.Edit";
        public const string Delete = "Per.Products.Delete";
        public const string Export = "Per.Products.Export";
    }
    [DisplayName("Shelfs")]
    public static class Shelfs
    {
        public const string View = "Per.Shelfs.View";
        public const string Create = "Per.Shelfs.Create";
        public const string Edit = "Per.Shelfs.Edit";
        public const string Delete = "Per.Shelfs.Delete";
        public const string Export = "Per.Shelfs.Export";
    }
    [DisplayName("ProductCategories")]
    public static class ProductCategories
    {
        public const string View = "Per.ProductCategories.View";
        public const string Create = "Per.ProductCategories.Create";
        public const string Edit = "Per.ProductCategories.Edit";
        public const string Delete = "Per.ProductCategories.Delete";
        public const string Export = "Per.ProductCategories.Export";
    }
    [DisplayName("Customers")]
    public static class Customers
    {
        public const string View = "Per.Customers.View";
        public const string Create = "Per.Customers.Create";
        public const string Edit = "Per.Customers.Edit";
        public const string Delete = "Per.Customers.Delete";
        public const string Export = "Per.Customers.Export";
    }
    [DisplayName("Questions")]
    public static class Questions
    {
        public const string View = "Per.Questions.View";
        public const string Create = "Per.Questions.Create";
        public const string Edit = "Per.Questions.Edit";
        public const string Delete = "Per.Questions.Delete";
        public const string Export = "Per.Questions.Export";
    }
    [DisplayName("Banners")]
    public static class Banners
    {
        public const string View = "Per.Banners.View";
        public const string Create = "Per.Banners.Create";
        public const string Edit = "Per.Banners.Edit";
        public const string Delete = "Per.Banners.Delete";
        public const string Export = "Per.Banners.Export";
    }
}
