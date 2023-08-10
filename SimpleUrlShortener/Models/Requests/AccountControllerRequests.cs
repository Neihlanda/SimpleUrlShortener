namespace SimpleUrlShortener.Models.Requests
{
    public record LoginRequest(string Login, string Password);

    public record RegisterRequest(string Login, string Password, string ConfirmPassword);
}
