using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SimpleUrlShortener.Data;
using SimpleUrlShortener.Models;
using SimpleUrlShortener.Models.Requests;

namespace SimpleUrlShortener.Tests.FunctionnalTests
{
    [Collection("IntegrationDatabase")]
    public class AccountControllerTests : IClassFixture<SimpleShortenerWebApplicationFactory>
    {
        private readonly SimpleShortenerWebApplicationFactory _factory;

        public AccountControllerTests(SimpleShortenerWebApplicationFactory webFactory)
        {
            _factory = webFactory;
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("Invalid", "")]
        [InlineData("", "Invalid")]
        [InlineData("Invalid", "Invalid")]
        public async Task Login_Endpoint_ReturnBadRequestIfUserLoginInvalid(string login, string password)
        {
            var client = _factory.CreateAnonymousHttpClient();
            var rqst = new LoginRequest(login, password);

            var response = await client.PostAsync("api/account/login", rqst.ToJsonHttpContent());
            var errorDetail = await response.Deserialize<ErrorDetails>();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(errorDetail);
            Assert.NotEmpty(errorDetail.Messages);
        }

        [Theory]
        [InlineData("", "", "")]
        [InlineData("Invalid", "", "")]
        [InlineData("", "Invalid", "Invalid")]
        [InlineData("Invalid", "Invalid", "")]
        public async Task SignIn_Endpoint_ReturnBadRequestIfSignInInvalid(string login, string password, string confirmPassword)
        {
            var client = _factory.CreateAnonymousHttpClient();
            var rqst = new RegisterRequest(login, password, confirmPassword);

            var response = await client.PostAsync("api/account/register", rqst.ToJsonHttpContent());
            var errorDetail = await response.Deserialize<ErrorDetails>();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(errorDetail);
            Assert.NotEmpty(errorDetail.Messages);
        }

        [Fact]
        public async Task SignIn_Endpoint_ReturnLoggedInResponseIfSignInValid()
        {
            var client = _factory.CreateAnonymousHttpClient();
            string password = Commons.StringUtility.RandomString(15);
            string login = Commons.StringUtility.RandomString(15);
            var rqst = new RegisterRequest(login, password, password);

            var response = await client.PostAsync("api/account/register", rqst.ToJsonHttpContent());
            var hasSetCookiesResp = response.Headers.TryGetValues("Set-Cookie", out var cookieValueList);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(hasSetCookiesResp);
            Assert.NotNull(cookieValueList);
            Assert.NotEmpty(cookieValueList);
        }

        [Fact]
        public async Task SignIn_Endpoint_MustRegisterUserIfSignInValid()
        {
            var client = _factory.CreateAnonymousHttpClient();
            string password = Commons.StringUtility.RandomString(15);
            string login = Commons.StringUtility.RandomString(15);
            var rqst = new RegisterRequest(login, password, password);

            var response = await client.PostAsync("api/account/register", rqst.ToJsonHttpContent());

            using var scope = _factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var createdUser = await userManager.FindByNameAsync(login);
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(createdUser);
        }
    }
}
