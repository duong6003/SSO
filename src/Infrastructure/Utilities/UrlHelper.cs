using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Utilities
{
    public static class UrlHelperExtention
    {
        // public static string EmailConfirmationLink(this IUrlHelper urlHelper,string controller ,Guid userId, string code, string scheme)
        // {
        //     return urlHelper.Action(
        //         action: controller,
        //         controller: "Users",
        //         values: new { userId, code },
        //         protocol: scheme)!;
        // }

        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string router , Guid id, string code, string scheme)
        {
            return urlHelper.ActionLink(
                action: router,
                controller: "Users",
                values: new { id, code },
                protocol: scheme)!;
        }
    }
}