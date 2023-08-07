using System;
using Microsoft.EntityFrameworkCore;

namespace SimpleUrlShortener.Commons
{
	public static class MiddlewareExtensions
	{
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
            => app.UseMiddleware<ExceptionMiddleware>();


        public static IApplicationBuilder ApplyDatabasePendingMigration(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            try
            {
                var appContext = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();

                logger.LogInformation("Début de l'application des migrations EF.");
                var pendingsMigrate = appContext.Database.GetPendingMigrations();

                if(pendingsMigrate.Any())
                {
                    logger.LogInformation($"{pendingsMigrate.Count()} migration(s) EF en attente.");
                    appContext.Database.Migrate();
                    logger.LogInformation("Migrations appliquées.");
                }

                logger.LogInformation("Fin de l'application des migrations EF.");
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Erreur lors de l'application des migrations EF.");
            }
            return app;
        }
    }
}

