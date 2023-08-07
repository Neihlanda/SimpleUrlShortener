using System;
using Microsoft.EntityFrameworkCore;
using SimpleUrlShortener.Data;
using SimpleUrlShortener.Models;

namespace SimpleUrlShortener.Services
{
    public class UrlShortenerService : IUrlShortenerService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebScrapperService _webScrapperService;

        public UrlShortenerService(AppDbContext appDbContext, IWebScrapperService webScrapper)
        {
            _appDbContext = appDbContext;
            _webScrapperService = webScrapper;
        }

        public async Task<ShortUrlDescription> CreateShortUrlDescription(ShortUrlDescription urlToCreate)
        {
            var validUri = await ValidateAndParseUrlToCreate(urlToCreate);
            var htmlHead = _webScrapperService.RetreiveHtmlHead(validUri);

            urlToCreate.Id = await GenerateNewShortUrlId();
            urlToCreate.AccessCount = 0;
            urlToCreate.ScrappedDescription = htmlHead.Description;
            urlToCreate.ScrappedTitle = htmlHead.Title;

            //simple règle, si utilisateur inscrit alors on garde plus longtemps la donnée
            int dayToAdd = string.IsNullOrEmpty(urlToCreate.OwnerId)
                ? ShortUrlDescription.ExpirationDelayUnkownOwner
                : ShortUrlDescription.ExpirationDelayKnownOwner;
            urlToCreate.ExpiredOn = DateTime.Now.Date.AddDays(dayToAdd);

            _appDbContext.ShortUrlDescriptions.Add(urlToCreate);
            await _appDbContext.SaveChangesAsync();

            return urlToCreate;
        }

        public async Task<ShortUrlDescription?> GetShortUrl(string id)
        {
            ShortUrlDescription? urlDescription = await _appDbContext.ShortUrlDescriptions.FindAsync(id);

            if(urlDescription != null)
            {
                if (urlDescription.UniqueAccess && urlDescription.AccessCount > 0)
                    return null;

                urlDescription.AccessCount++;
                await _appDbContext.SaveChangesAsync();
            }
            return urlDescription;
        }

        public async Task<IEnumerable<ShortUrlDescription>> GetUserShortUrlDescription(string userId)
        {
            return await _appDbContext.ShortUrlDescriptions
                    .Where(x => x.OwnerId == userId)
                    .ToListAsync();
        }

        public async Task PurgeExpiredDescription()
        {
            var expiredUrls = _appDbContext.ShortUrlDescriptions
                .Where(a => a.ExpiredOn < DateTime.Now.Date);

            if (await expiredUrls.AnyAsync())
            {
                _appDbContext.ShortUrlDescriptions.RemoveRange(await expiredUrls.ToListAsync());
                await _appDbContext.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateNewShortUrlId()
        {
            string randomId = string.Empty;
            bool idAlreadyUsed = false;
            do
            {
                randomId = Commons.StringUtility.RandomString(ShortUrlDescription.IdMaxLength);
                idAlreadyUsed = await _appDbContext.ShortUrlDescriptions.AnyAsync(a => a.Id == randomId);
            }
            while (idAlreadyUsed);

            return randomId;
        }

        private async Task<Uri> ValidateAndParseUrlToCreate(ShortUrlDescription urlToCreate)
        {
            ArgumentNullException.ThrowIfNull(urlToCreate, nameof(urlToCreate));
            Uri? validUri = null;

            if (string.IsNullOrEmpty(urlToCreate.DestinationUrl))
                throw new ArgumentException("L'url ne peut pas être vide.");

            if (!string.IsNullOrEmpty(urlToCreate.OwnerId) && !(await _appDbContext.Users.AnyAsync(p => p.Id == urlToCreate.OwnerId)))
                throw new ArgumentException("L'identifiant de l'utilisateur est inconnu.");

            try
            {
                validUri = new Uri(urlToCreate.DestinationUrl);
                return validUri;
            }
            catch (UriFormatException ex)
            {
                throw new ArgumentException("L'url n'est pas valide.", ex);
            }
        }
    }
}
