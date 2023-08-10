using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleUrlShortener.Commons;
using SimpleUrlShortener.Models.Requests;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;

namespace SimpleUrlShortener.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpPost("Login"), AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.Login);
            bool authIsSuccess = false;
            if (user != null)
            {
                var signedIn = await _signInManager.PasswordSignInAsync(user, loginRequest.Password, false, true);
                authIsSuccess = signedIn.Succeeded;
            }

            if (!authIsSuccess)
                throw SimpleUrlShortener.Commons.AuthenticationException.LoginRequestInvalid;

            return Ok();
        }

        [HttpPost("Logout"), Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpGet("Me"), AllowAnonymous]
        public IActionResult UserClaims()
        {
            Dictionary<string, dynamic> claims = new();
            if (User?.Claims?.Any() == true)
            {
                foreach (var claimType in User.Claims.GroupBy(t => t.Type))
                {
                    JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.TryGetValue(claimType.Key, out string? shortName);
                    claims[shortName ?? claimType.Key] = claimType.Count() == 1 ? claimType.First().Value : claimType.Select(a => a.Value).ToList();
                }
            }
          
            return Ok(claims);
        }

        [HttpPost("Register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null)
                throw new ArgumentException("Formulaire d'inscription invalide.");

            if (string.IsNullOrEmpty(registerRequest.Password))
                throw new ArgumentException("Le mot de passe ne peut pas être vide !");

            if (string.IsNullOrEmpty(registerRequest.Login))
                throw new ArgumentException("Le nom d'utilisateur ne peut pas être vide !");

            if (registerRequest.Password != registerRequest.ConfirmPassword)
                throw new ArgumentException("Le mot de passe et sa confirmation ne correspondent pas !");


            var result = await _userManager.CreateAsync(new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = registerRequest.Login,
                Email = registerRequest.Login
            }, registerRequest.Password);

            if (result.IsSucceeded())
            {
                return await Login(new LoginRequest (registerRequest.Login, registerRequest.Password));
            }

            return BadRequest();
        }
    }
}
