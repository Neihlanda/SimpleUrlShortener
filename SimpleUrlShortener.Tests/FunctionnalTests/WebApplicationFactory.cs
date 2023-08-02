using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SimpleUrlShortener.Data;
using SimpleUrlShortener.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUrlShortener.Tests.FunctionnalTests
{
    [Collection("IntegrationDatabase")]
    internal class SimpleShortenerWebApplicationFactory : WebApplicationFactory<Program>
    {

        static readonly string FunctionnalTestDb = Path.GetTempFileName();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                SetSqliteDbContext<AppDbContext>(services, FunctionnalTestDb);
            });
        }

        private static void SetSqliteDbContext<T>(IServiceCollection services, string filenameDb) where T : DbContext
        {
            var descriptor = services.Single(
                d => d.ServiceType ==
                    typeof(DbContextOptions<T>));
            services.Remove(descriptor);

            var connectionString = $"Filename={filenameDb}";
            services.AddDbContext<T>(options =>
                options.UseSqlite(connectionString)
            );
        }

        public async Task<HttpClient> CreateLoggedInHttpClient()
        {
            var client = this.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true,
            });


            var (user, pwd) = await CreateUser();
            var LoginRsqt = new LoginRequest(user.UserName!, pwd);

            var response = await client.PostAsync("api/account/login", LoginRsqt.ToJsonHttpContent());
            if (response.Headers.TryGetValues("Set-Cookie", out var cookieValueList))
            {
                var cookieContainer = new CookieContainer();
                foreach (var value in cookieValueList)
                {
                    cookieContainer.SetCookies(client.BaseAddress, value);
                }
                foreach (Cookie? cookie in cookieContainer.GetAllCookies())
                {
                    client.DefaultRequestHeaders.Add(CookieAuthenticationDefaults.AuthenticationScheme, cookie?.ToString());
                }
            }

            return client;
        }

        public HttpClient CreateAnonymousHttpClient()
        {
            return this.CreateClient();
        }

        private async Task<(IdentityUser FakeUser, string FakeUserPassword)> CreateUser()
        {
            using var scope = this.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            var (user, password) = GenerateFakeUser();

            await userManager.CreateAsync(user, password);
            return (user, password);
        }

        internal static (IdentityUser FakeUser, string FakeUserPassword) GenerateFakeUser()
        {

            var fakeUser = new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = TestHelper.RandomString(10),
            };
            var fakeUserPassword = TestHelper.RandomString(15);
            return (fakeUser, fakeUserPassword);
        }
    }

    [CollectionDefinition("IntegrationDatabase")]
    public class DatabaseCollection : ICollectionFixture<SimpleShortenerWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }
}
