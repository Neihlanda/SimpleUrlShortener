using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleUrlShortener.Commons;
using SimpleUrlShortener.Models.Requests;
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

        [HttpPost("SignIn"), AllowAnonymous]
        public async Task<IActionResult> SignIn([FromBody] SigninRequest signinRequest)
        {
            if (signinRequest == null)
                throw new ArgumentException("Formulaire d'inscription invalide.");

            if (string.IsNullOrEmpty(signinRequest.Password))
                throw new ArgumentException("Le mot de passe ne peut pas être vide !");

            if (string.IsNullOrEmpty(signinRequest.Login))
                throw new ArgumentException("Le nom d'utilisateur ne peut pas être vide !");

            if (signinRequest.Password != signinRequest.ConfirmPassword)
                throw new ArgumentException("Le mot de passe et sa confirmation ne correspondent pas !");


            var result = await _userManager.CreateAsync(new IdentityUser()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = signinRequest.Login,
                Email = signinRequest.Login
            }, signinRequest.Password);

            if (result.IsSucceeded())
            {
                return await Login(new LoginRequest (signinRequest.Login, signinRequest.Password));
            }

            return BadRequest();
        }
    }
}
