using System;
using Microsoft.EntityFrameworkCore;
using System.Net;
using SimpleUrlShortener.Models;
using System.Text;

namespace SimpleUrlShortener.Commons
{
	public class ExceptionMiddleware
	{
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next,
             ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(httpContext, ex, _logger);
            }
        }

        public async Task HandleException(HttpContext context, Exception exception, ILogger logger)
        {
            ErrorDetails errorDetails = new()
            {
                Source = exception.TargetSite?.DeclaringType?.FullName,
                Exception = exception.Message.Trim(),
                Messages = new List<string>(),
                StatusCode = (int)HttpStatusCode.InternalServerError
            };

            switch (exception)
            {
                case KeyNotFoundException nfException:
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    errorDetails.Messages.Add(nfException.Message ?? "La ressource demandée est introuvable.");
                    break;

                case ArgumentNullException:
                case ArgumentException:
                case AppException:
                case InvalidOperationException:
                case AuthenticationException _:
                    errorDetails.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorDetails.Messages.Add(exception.Message);
                    if (exception.Data.Contains(IdentityResultExtension.IdentityErrorExceptionDataKey)
                        && exception.Data[IdentityResultExtension.IdentityErrorExceptionDataKey] is IEnumerable<string> errors)
                    {
                        errorDetails.Messages.AddRange(errors);
                    }
                    break;

                case UnauthorizedAccessException _:
                    errorDetails.StatusCode = (int)HttpStatusCode.NotFound;
                    errorDetails.Messages.Add(exception.Message ?? "Accès non autorisé");
                    logger.LogWarning(exception, "Tentative d'accès non autorisées");
                    break;

                default:
                    errorDetails.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorDetails.Messages.Add("Une erreur inconnue est survenue.");
                    logger.LogError(exception, "Erreur inconnue");
                    break;
            }

            var response = context.Response;
            if (!response.HasStarted)
            {
                context.Response.ContentType = "application/json";
                response.StatusCode = errorDetails.StatusCode;
                await context.Response.WriteAsJsonAsync(errorDetails);
            }
            else
            {
                logger.LogWarning("HttpResponse déjà traitée, impossbile d'override le retour.");
            }
        }
    }
}

