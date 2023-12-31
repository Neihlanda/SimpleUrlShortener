﻿namespace SimpleUrlShortener.Commons
{
    public class AppException : Exception
    {
        public AppException(string? message) : base(message)
        {
        }

        public AppException(string? message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class AuthenticationException : Exception
    {
        internal static AuthenticationException LoginRequestInvalid = new("Nom d'utilisateur et/ou mot de passe incorrect.");

        public AuthenticationException()
        {
        }

        public AuthenticationException(string message) : base(message)
        {
        }

        public AuthenticationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class WebScrapperException : AppException
    {
        public WebScrapperException(string? message) : base(message)
        {
        }

        public WebScrapperException(string? message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class AppIdentityException : AppException
    {
        public AppIdentityException(string? message) : base(message)
        {
        }

        public AppIdentityException(string? message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
