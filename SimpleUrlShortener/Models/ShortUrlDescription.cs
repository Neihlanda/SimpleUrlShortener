namespace SimpleUrlShortener.Models
{
    public class ShortUrlDescription
    {
        public const int IdMaxLength = 9;
        public const int ExpirationDelayUnkownOwner = 7;
        public const int ExpirationDelayKnownOwner = 30;


        public string Id { get; set; }

        public required string DestinationUrl { get; set; }

        public string? ScrappedTitle { get; set; }

        public string? ScrappedDescription { get; set; }

        public long AccessCount { get; set; }

        public DateTime ExpiredOn { get; set; }

        public bool UniqueAccess { get; set; }

        public string? OwnerId { get; set; }
    }
}
