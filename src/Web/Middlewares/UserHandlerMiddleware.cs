// using Infrastructure.Definitions;
// using Infrastructure.Persistence.Repositories;
// using Microsoft.EntityFrameworkCore;
// using Newtonsoft.Json;
// using System.Net;
// using System.Security.Claims;

// namespace Web.Middlewares
// {
//     // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
//     public class UserHandlerMiddleware
//     {
//         private readonly RequestDelegate _next;
//         private readonly IServiceScopeFactory _serviceScopeFactory;

//         public UserHandlerMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
//         {
//             _next = next;
//             _serviceScopeFactory = serviceScopeFactory;
//         }

//         public async Task Invoke(HttpContext httpContext)
//         {
//             if (httpContext.User.Identity!.IsAuthenticated)
//             {
//                 string userId = httpContext.User.FindFirstValue(JwtClaimsName.Identification);
//                 if (userId == null && RoutePath.UserRoutePath.Any(y => httpContext.Request.Path.Value!.Equals(y.Key) && httpContext.Request.Method.Equals(y.Value)))
//                 {
//                     await HandleResponse(httpContext, "UnAuthorized");
//                 }
//                 else if (userId != null)
//                 {
//                     await _next(httpContext);
//                 }
//                 else
//                 {
//                     string accountId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
//                     List<string> currentRole = httpContext.User.FindAll(ClaimTypes.Role).Select(s => s.Value).ToList();
//                     using IServiceScope serviceScope = _serviceScopeFactory.CreateScope();
//                     IRepositoryWrapper RepositoryWrapper = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapper>();
//                     IAreaService AreaService = serviceScope.ServiceProvider.GetRequiredService<IAreaService>();
//                     Account? account = await RepositoryWrapper.Accounts
//                         .Find(x => x.AccountId!.Equals(accountId))
//                         .Include(x => x.Area)
//                         .ThenInclude(x => x!.ParentArea)
//                         .Include(x => x.Role)
//                         .ThenInclude(x => x!.RolePermissions)
//                         .FirstOrDefaultAsync();
//                     if (account is null)
//                     {
//                         await HandleResponse(httpContext, "UserDoesNotExists");
//                     }
//                     else if (account.Area is not null)
//                     {
//                         Area? areaRoot = await AreaService.GetRoot(account.AreaId);
//                         if (account.Area.AreaStatus == AreaStatus.Deactive)
//                         {
//                             await HandleResponse(httpContext, "AreaDeactive");
//                         }
//                         else if (areaRoot is not null)
//                         {
//                             if (areaRoot.LicenseExpirationAt is not null && areaRoot.LicenseExpirationAt < DateTime.Now)
//                             {
//                                 await HandleResponse(httpContext, "LicenseExpirationAtHasExpired");
//                             }
//                             else
//                             {
//                                 await _next(httpContext);
//                             }
//                         }
//                         else
//                         {
//                             await _next(httpContext);
//                         }
//                     }
//                     else if (await RepositoryWrapper.AreaRelations.AnyAsync(x => x.ChildId!.Equals(account.AreaId) && x.Parent!.AreaStatus == AreaStatus.Deactive))
//                     {
//                         await HandleResponse(httpContext, "ParentAreaDeactive");
//                     }
//                     else if (account.Role is not null && account.Role.RolePermissions!.Count != currentRole.Count)
//                     {
//                         await HandleResponse(httpContext, "RoleIsChange");
//                     }
//                     else
//                     {
//                         await _next(httpContext);
//                     }
//                 }
//             }
//             else
//             {
//                 await _next(httpContext);
//             }

//         }
//         private async Task HandleResponse(HttpContext httpContext, string message)
//         {
//             httpContext.Response.ContentType = "application/json";
//             httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//             await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { message = message }));
//         }
//     }

//     // Extension method used to add the middleware to the HTTP request pipeline.
//     public static class MiddlewareExtensions
//     {
//         public static IApplicationBuilder UseUserHandlerMiddleware(this IApplicationBuilder builder)
//         {
//             return builder.UseMiddleware<UserHandlerMiddleware>();
//         }
//     }
// }
