using SimpleUrlShortener.Models;
using SimpleUrlShortener.Models.Requests;

namespace SimpleUrlShortener.Services
{
    public interface IUrlShortenerService
    {
        public Task<ShortUrlDescription> GetShortUrl(string id);

        public Task<ShortUrlDescription> CreateShortUrlDescription(ShortUrlDescription urlToCreate);

        public Task<IEnumerable<ShortUrlDescription>> GetUserShortUrlDescription(string userId);

        public Task PurgeExpiredDescription();
    }
}
