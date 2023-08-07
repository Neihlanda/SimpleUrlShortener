using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SimpleUrlShortener.Commons;
using SimpleUrlShortener.Data;
using SimpleUrlShortener.Models;
using SimpleUrlShortener.Models.Requests;

namespace SimpleUrlShortener.Tests.FunctionnalTests
{
    [Collection("IntegrationDatabase")]
    public class UrlShortenerControllerTests : IClassFixture<SimpleShortenerWebApplicationFactory>
    {
        private readonly SimpleShortenerWebApplicationFactory _factory;

        public UrlShortenerControllerTests(SimpleShortenerWebApplicationFactory webFactory)
        {
            _factory = webFactory;
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("       ")]
        [InlineData("invalidurl")]
        [InlineData("invalidur@email.fr")]
        public async Task Post_Endpoint_ReturnBadRequestIfInvalid(string? invalidUrl)
        {
            var client = _factory.CreateAnonymousHttpClient();
            var rqst = new CreateShortUrlRequest(invalidUrl, false);

            var response = await client.PostAsync("api/urlshortener", rqst.ToJsonHttpContent());
            var errorDetail = await response.Deserialize<ErrorDetails>();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotNull(errorDetail);
            Assert.NotEmpty(errorDetail.Messages);
        }

        [Theory]
        [InlineData("https://w3schools.com")]
        [InlineData("http://wikipedia.org")]
        [InlineData("https://google.com")]
        [InlineData("http://google.com")]
        public async Task Post_Endpoint_ReturnCreatedShortUrlDescription(string validUrl)
        {
            var client = _factory.CreateAnonymousHttpClient();
            var rqst = new CreateShortUrlRequest(validUrl, false);

            var response = await client.PostAsync("api/urlshortener", rqst.ToJsonHttpContent());
            var description = await response.Deserialize<ShortUrlDescription>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(description);
            Assert.Equal(ShortUrlDescription.IdMaxLength, description.Id.Length);
            Assert.Equal(validUrl, description.DestinationUrl);
            Assert.Equal(0, description.AccessCount);
        }

        [Fact]
        public async Task Post_Endpoint_ReturnLargeExpirationShortUrlDescription_WithLoggedInUser()
        {
            var client = await _factory.CreateLoggedInHttpClient();
            var rqst = new CreateShortUrlRequest("https://google.com", false);

            var response = await client.PostAsync("api/urlshortener", rqst.ToJsonHttpContent());
            var description = await response.Deserialize<ShortUrlDescription>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(description);
            Assert.True(description.ExpiredOn == DateTime.Now.Date.AddDays(ShortUrlDescription.ExpirationDelayKnownOwner));
        }

        [Fact]
        public async Task Post_Endpoint_ReturnShortExpirationShortUrlDescription_WithAnonymousUser()
        {
            var client = _factory.CreateAnonymousHttpClient();
            var rqst = new CreateShortUrlRequest("https://google.com", false);

            var response = await client.PostAsync("api/urlshortener", rqst.ToJsonHttpContent());
            var description = await response.Deserialize<ShortUrlDescription>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(description);
            Assert.True(description.ExpiredOn == DateTime.Now.Date.AddDays(ShortUrlDescription.ExpirationDelayUnkownOwner));
        }

        [Fact]
        public async Task Get_Endpoint_ReturnNotFoundIfNotExit()
        {
            var client = _factory.CreateAnonymousHttpClient();

            var response = await client.GetAsync($"api/urlshortener/unexistingId");
            var errorDetails = await response.Deserialize<ErrorDetails>();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(errorDetails);
            Assert.NotEmpty(errorDetails.Messages);
        }

        [Fact]
        public async Task Get_Endpoint_ReturnShortDescriptionWithIncrementedCount()
        {
            var dbDescription = await CreateFakeShortDescription(false);
            var client = _factory.CreateAnonymousHttpClient();

            var response = await client.GetAsync($"api/urlshortener/{dbDescription.Id}");
            var description = await response.Deserialize<ShortUrlDescription>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(description);
            Assert.Equal(dbDescription.Id, description.Id);
            Assert.Equal(dbDescription.AccessCount, description.AccessCount - 1);
        }

        [Fact]
        public async Task Get_Endpoint_ReturnNotFoundIfUniqueAccessConsumed()
        {
            var dbDescription = await CreateFakeShortDescription(true);
            var client = _factory.CreateAnonymousHttpClient();

            var okResponse = await client.GetAsync($"api/urlshortener/{dbDescription.Id}");
            var description = await okResponse.Deserialize<ShortUrlDescription>();
            var notFoundResponse = await client.GetAsync($"api/urlshortener/{dbDescription.Id}");
            var notFoundError = await notFoundResponse.Deserialize<ErrorDetails>();

            Assert.Equal(HttpStatusCode.OK, okResponse.StatusCode);
            Assert.NotNull(description);
            Assert.Equal(1, description.AccessCount);
            Assert.Equal(HttpStatusCode.NotFound, notFoundResponse.StatusCode);
            Assert.NotNull(notFoundError);
            Assert.NotEmpty(notFoundError.Messages);
        }

        [Fact]
        public async Task GetCreatedByMe_Endpoint_ReturnMyUrlWithoutIncrementAccessCount()
        {
            var client = await _factory.CreateLoggedInHttpClient();
            var urlToCreate = new string[]
            {
                "https://google.com",
                "https://elgoog.im"
            };

            foreach(var url in urlToCreate)
            {
                var rqst = new CreateShortUrlRequest(url, false);
                _ = await client.PostAsync("api/urlshortener", rqst.ToJsonHttpContent());
            }
            var myUrlsResponse = await client.GetAsync("api/urlshortener/CreatedByMe");
            var myCreatedUrls = await myUrlsResponse.Deserialize<List<ShortUrlDescription>>();

            Assert.Equal(HttpStatusCode.OK, myUrlsResponse.StatusCode);
            Assert.NotNull(myCreatedUrls);
            Assert.Equal(urlToCreate.Length, myCreatedUrls.Count);
            Assert.True(myCreatedUrls.All(p => p.AccessCount == 0));
        }

        private async Task<ShortUrlDescription> CreateFakeShortDescription(bool uniqueAccess)
        {
            using var scope = _factory.Services.CreateScope();
            var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var shortUrlDesc = new ShortUrlDescription
            {
                DestinationUrl = "https://github.com",
                AccessCount = 0,
                ExpiredOn = DateTime.Now.AddDays(ShortUrlDescription.ExpirationDelayUnkownOwner),
                UniqueAccess = uniqueAccess,
                Id = StringUtility.RandomString(ShortUrlDescription.IdMaxLength)
            };

            appDbContext.ShortUrlDescriptions.Add(shortUrlDesc);
            await appDbContext.SaveChangesAsync();

            return shortUrlDesc;
        }
    }
}
