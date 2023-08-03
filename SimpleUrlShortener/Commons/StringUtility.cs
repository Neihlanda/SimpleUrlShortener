using System;

namespace SimpleUrlShortener.Commons
{
	public static class StringUtility
	{
        static readonly Random random = new();

        public static string RandomString(int lenght)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, lenght).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}

