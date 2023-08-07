using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleUrlShortener.Models.Requests;
using SimpleUrlShortener.Services;

namespace SimpleUrlShortener.Controllers
{

    [Route("api/[controller]")]
    public class UrlShortenerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUrlShortenerService _urlShortenerService;

        public UrlShortenerController(UserManager<IdentityUser> userManager, IUrlShortenerService urlShortenerService) 
        { 
            _userManager = userManager;
            _urlShortenerService = urlShortenerService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetShortUrlDescription(string id)
        {
            var description = await _urlShortenerService.GetShortUrl(id);
            return description == null
                ? throw new KeyNotFoundException("L'URL demandée est introuvable.")
                : Ok(description);            
        }

        [HttpPost]
        public async Task<IActionResult> CreateShortUrl([FromBody]CreateShortUrlRequest request)
        {
            string? userId = null;
            if(User != null)
            {
                var loggedInUser = await _userManager.GetUserAsync(User);
                userId = loggedInUser?.Id;
            }

            return Ok(await _urlShortenerService.CreateShortUrlDescription(new Models.ShortUrlDescription()
            {
                DestinationUrl = request.UrlToProcess,
                UniqueAccess = request.UniqueUsage,
                OwnerId = userId
            }));
        }
        
        [HttpGet("CreatedByMe"), Authorize]
        public async Task<IActionResult> GetMyUrl()
        {
            var loggedInUser = await _userManager.GetUserAsync(User);
            if (loggedInUser == null)
                return NotFound();

            return Ok(await _urlShortenerService.GetUserShortUrlDescription(loggedInUser.Id));
        }
    }
}
