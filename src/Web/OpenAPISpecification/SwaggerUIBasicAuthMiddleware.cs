using System.Net;
using System.Text;

namespace Web.OpenAPISpecification
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class SwaggerUIBasicAuthMiddleware
    {
        private readonly RequestDelegate RequestDelegate;
        private readonly IConfiguration Configuration;

        public SwaggerUIBasicAuthMiddleware(RequestDelegate requestDelegate, IConfiguration configuration)
        {
            RequestDelegate = requestDelegate;
            Configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext.Request.Path.StartsWithSegments("/swagger"))
            {
                if
                (
                    !httpContext.Request.Headers.ContainsKey("Authorization")
                    || !httpContext.Request.Headers["Authorization"].First().Split(" ").Last()!.Equals(Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Configuration!["SwaggerSettings:Username"]}:{Configuration!["SwaggerSettings:Password"]}")))
                )
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    httpContext.Response.Headers.Add("WWW-Authenticate", "Basic");
                }
                else
                {
                    await RequestDelegate(httpContext);
                }
            }
            else
            {
                await RequestDelegate(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class SwaggerUIBasicAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseSwaggerUIBasicAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerUIBasicAuthMiddleware>();
        }
    }
}
