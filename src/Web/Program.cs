using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Serilog;
using System.Reflection;
using System.Text;
using Web.OpenAPISpecification;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Information("ServerStartingUp...");

try
{
    var builder = WebApplication.CreateBuilder(args);

    #region Add Configuration
    builder.Host.ConfigureAppConfiguration((context, configureDelegate) =>
    {
        configureDelegate.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables(prefix: "MXM_");
    });
    IConfiguration Configuration = builder.Configuration;
    #endregion

    #region Add Health Checks
    builder.Services.AddHealthChecks().AddMySql
    (
        connectionString: Configuration["DatabaseSettings:MySQLSettings:ConnectionStrings:DefaultConnection"]
    )
    .AddUrlGroup(new Uri("https://httpstatuses.com/200"));

    builder.Services.AddHealthChecksUI().AddInMemoryStorage();
    #endregion

    #region Add Serilog
    builder.Host.UseSerilog((_, configureLogger) =>
    {
        configureLogger.ReadFrom.Configuration(Configuration);
    });
    #endregion

    #region Add Jwt Bearer
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = Configuration["JwtSettings:Issuer"],
            ValidAudience = Configuration["JwtSettings:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JwtSettings:SecretKey"]))
        };
    });
    #endregion

    #region Add Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            BearerFormat = "JWT",
            Description = "Input your Bearer token to access this API",
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http
        });

        options.OperationFilter<SecurityRequirementsOperationFilter>();
        // using System.Reflection;
        var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    });
    #endregion

    #region Add Cors Policy
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowSpecificOrigin", builder =>
        {
            List<string> allowedOrigins = new();
            if (Configuration.GetSection($"AllowedOrigins").Exists() && !string.IsNullOrEmpty(Configuration.GetSection($"AllowedOrigins").Get<string>()) && !Configuration.GetSection($"AllowedOrigins").Get<string>().Split(";").Any(x => x.Equals("*")))
            {
                allowedOrigins.AddRange(Configuration.GetSection($"AllowedOrigins").Get<string>().Split(";"));
            }
            else
            {
                allowedOrigins.Add("*");
            }
            builder.SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithOrigins(allowedOrigins.ToArray())
                    .AllowAnyMethod()
                    .AllowAnyHeader();
        });
    });
    #endregion

    #region Add Infrastructure Configure Services
    builder.Services.AddInfrastructureConfigureServices(Configuration);
    #endregion

    #region Add Extensions
    builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            return new BadRequestObjectResult(new { message = context.ModelState?.FirstOrDefault(x => x.Value.ValidationState is ModelValidationState.Invalid).Value?.Errors[0].ErrorMessage });
        };
    })


    .AddFluentValidation(options =>
    {
        options.RegisterValidatorsFromAssemblyContaining(typeof(Startup));
    })
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    });
    #endregion

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwaggerUIBasicAuthMiddleware();
    }
    app.UseSwagger();

    app.UseSwaggerUI(configure =>
    {
        configure.ConfigObject.PersistAuthorization = true;
    });

    app.UseCors("AllowSpecificOrigin");

    //app.UseInfrastructureConfigure(Configuration);


    app.UseHttpsRedirection();

    app.UseRouting();

    app.UseAuthentication();

    app.UseAuthorization();


    app.UseEndpoints(configure =>
    {
        configure.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        configure.MapHealthChecksUI(setupOptions =>
        {
            setupOptions.AddCustomStylesheet("HealthChecks/CustomStyle.css");
        });

        configure.MapControllers();
    });

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "UnhandledException");
}
finally
{
    Log.Information("ServerShuttingDown...");
    Log.CloseAndFlush();
}
