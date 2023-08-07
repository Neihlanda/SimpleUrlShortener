using SimpleUrlShortener.Commons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services
    .AddAppDbContext(builder.Configuration)
    .AddAppServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseGlobalExceptionHandler()
   .UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseCors(a => a.SetIsOriginAllowed(a => a.Contains("localhost")).AllowAnyHeader().AllowAnyMethod().AllowCredentials());
}

app.UseAuthentication()
   .UseAuthorization();

app.MapControllers();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.ApplyDatabasePendingMigration();

app.Run();

//déclaration de la classe ici pour manipulation dans le projet de test.
public partial class Program { }
