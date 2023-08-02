using SimpleUrlShortener.Data;
using SimpleUrlShortener.Models;

namespace SimpleUrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly AppDbContext _appDbContext;

        public UrlShortenerService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task<ShortUrlDescription> CreateShortUrlDescription(ShortUrlDescription urlToCreate)
        {
            //TODO Scrapper web + créer un id sur 9 caractère random (vérifier l'existance en base)
            throw new NotImplementedException();
        }

        public Task<ShortUrlDescription> GetShortUrl(string id)
        {
            //TODO increment access, levé erreur si acces unique et access count > 0
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ShortUrlDescription>> GetUserShortUrlDescription(string userId)
        {
            //TODO ne pas incrémenter l'access count, prévu pour "dashboard"
            throw new NotImplementedException();
        }

        public Task PurgeExpiredDescription()
        {
            //TODO Créer un background service pour la purge des données
            throw new NotImplementedException();
        }
    }
}
