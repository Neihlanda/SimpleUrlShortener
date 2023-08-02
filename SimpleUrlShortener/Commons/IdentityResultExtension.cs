using Microsoft.AspNetCore.Identity;

namespace SimpleUrlShortener.Commons
{
    public static class IdentityResultExtension
    {
        public const string IdentityErrorExceptionDataKey = nameof(IdentityErrorExceptionDataKey);

        /// <summary>
        /// Si pas <see cref="IdentityResult.Succeeded"/>, léve une erreur récupérant la totalité des erreurs présentes dans <see cref="IdentityResult.Errors"/>
        /// pour être traitées dans le middleware
        /// </summary>
        /// <param name="result"></param>t
        /// <returns></returns>
        public static bool IsSucceeded(this IdentityResult result)
        {
            if (!result.Succeeded)
            {
                var argEx = new ArgumentException("L'opération est invalide.");
                argEx.Data[IdentityErrorExceptionDataKey] = string.Join(' ', result.Errors.Select(p => p.Description));
                throw argEx;
            }
            return result.Succeeded;
        }
    }
}
