using System;
namespace SimpleUrlShortener.Models
{
	public class ErrorDetails
	{
        public List<string> Messages { get; set; } = new();

        public string? Source { get; set; }

        public string? Exception { get; set; }

        public int StatusCode { get; set; }
    }
}

