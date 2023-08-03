using System;

namespace SimpleUrlShortener.Commons
{
	public class ExceptionMiddleware : IMiddleware
	{
		public ExceptionMiddleware()
		{
		}

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //TODO Finaliser ici, implémenter les exceptions métiers perso + retour http & model d'erreur custom
            throw new NotImplementedException();
        }
    }
}

