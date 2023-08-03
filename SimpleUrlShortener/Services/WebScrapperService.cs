using System;
using HtmlAgilityPack;
using SimpleUrlShortener.Commons;
using SimpleUrlShortener.Models;

namespace SimpleUrlShortener.Services
{
    public class WebScrapperService : IWebScrapperService
    {
        public HtmlHead RetreiveHtmlHead(Uri uri)
        {
            try
            {
                HtmlWeb web = new();
                var htmlDoc = web.Load(uri);

                var title = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
                var metaDescription = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");

                return new HtmlHead()
                {
                    Title = title?.InnerHtml,
                    Description = metaDescription?.GetAttributeValue("content", null)
                };
            }
            catch(HtmlWebException webEx)
            {
                throw new WebScrapperException("Impossible de charger les métadonnées de la page cible.", webEx);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

