using Core.Common.Interfaces;
using Hangfire;
using Hangfire.MySql;
using HangfireBasicAuthenticationFilter;
using Infrastructure.Mappings;
using Infrastructure.Persistence.Contexts;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Utilities;

namespace Infrastructure;

public static class Startup
{
    public static IServiceCollection AddInfrastructureConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireServer(options => configuration.GetSection("HangfireSettings:Server").Bind(options));

        services.AddHangfire(hangfireConfiguration => hangfireConfiguration.UseStorage(
            new MySqlStorage(
                configuration["HangfireSettings:Storage:ConnectionString"],
                configuration.GetSection("HangfireSettings:Storage:Options").Get<MySqlStorageOptions>()
            )
        ));

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseMySql(configuration["DatabaseSettings:MySQLSettings:ConnectionStrings:DefaultConnection"],
            ServerVersion.AutoDetect(configuration["DatabaseSettings:MySQLSettings:ConnectionStrings:DefaultConnection"])
        ));

        services.AddAutoMapper(typeof(MappingProfile));
        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

        #region Add Redis Service
        services.AddSingleton<IRedisDatabaseProvider, RedisDBContext>();
        #endregion

        #region Add Service Lifetime
        services.AddServices();
        services.AddScoped<IAmazonS3Utility, AmazonS3Utility>();
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        #endregion

        return services;
    }

    public static IApplicationBuilder UseInfrastructureConfigure(this IApplicationBuilder app, IConfiguration configuration)
    {
        DashboardOptions dashboardOptions = configuration.GetSection("HangfireSettings:Dashboard").Get<DashboardOptions>();
        dashboardOptions.Authorization = new[]
        {
            new HangfireCustomBasicAuthenticationFilter
            {
                User = configuration["HangfireSettings:Credentials:Username"],
                Pass = configuration["HangfireSettings:Credentials:Password"]
            }
        };
        app.UseHangfireDashboard(configuration["HangfireSettings:Route"], dashboardOptions);

        return app;
    }

    internal static IServiceCollection AddServices(this IServiceCollection services) => services.AddServices(typeof(ITransientService), ServiceLifetime.Transient).AddServices(typeof(IScopedService), ServiceLifetime.Scoped);

    internal static IServiceCollection AddServices(this IServiceCollection services, Type interfaceType, ServiceLifetime lifetime)
    {
        var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
            .Select(t => new
            {
                Service = t.GetInterfaces().FirstOrDefault(),
                Implementation = t
            })
            .Where(t => t.Service is not null && interfaceType.IsAssignableFrom(t.Service));

        foreach (var type in interfaceTypes)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(type.Service!, type.Implementation);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(type.Service!, type.Implementation);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(type.Service!, type.Implementation);
                    break;
                default:
                    throw new ArgumentException("InvalidLifetime", nameof(lifetime));
            }
        }

        return services;
    }
}
