using Newtonsoft.Json;
using Serilog;
using System.Net;

namespace Web.Exceptions;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate RequestDelegate;

    public ExceptionHandlerMiddleware(RequestDelegate requestDelegate)
    {
        RequestDelegate = requestDelegate;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        try
        {
            await RequestDelegate(httpContext);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "ExceptionHandlerMiddleware");
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(new { message = ex.GetBaseException().ToString() }));
        }
    }
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class ExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlerMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionHandlerMiddleware>();
    }
}
