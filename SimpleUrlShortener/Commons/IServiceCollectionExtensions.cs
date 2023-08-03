using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SimpleUrlShortener.Data;
using System;
using System.Data;
using System.Reflection;

namespace SimpleUrlShortener.Commons
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(opt => opt.UseSqlite(configuration.GetConnectionString("AppDb")).EnableSensitiveDataLogging(true))
                .AddIdentity<IdentityUser, IdentityRole>(opt =>
                {
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequiredUniqueChars = 3;
                    opt.Password.RequireLowercase = false;
                })

                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(opt =>
            {
                opt.Cookie.SameSite = SameSiteMode.Strict;
            });

            return services;
        }



        public static IServiceCollection AddAppServices(this IServiceCollection services)
        {
            var ServiceInterfaceType = Assembly.GetExecutingAssembly()
                            .GetTypes()
                            .Where(t => t.IsInterface && t.Name.EndsWith("Service"))
                            .ToArray();
            foreach (var implementedInterface in ServiceInterfaceType)
            {
                var ImplementedType = Assembly.GetExecutingAssembly().GetTypes().First(t => t.IsClass && implementedInterface.IsAssignableFrom(t));
                services.AddScoped(implementedInterface, ImplementedType);
            }

            services.AddHostedService<PurgeExpiredDescriptionBackgroundService>();

            return services;
        }
    }
}
