namespace SimpleUrlShortener.Models.Requests
{
    public record CreateShortUrlRequest(string UrlToProcess, bool UniqueUsage);
}
