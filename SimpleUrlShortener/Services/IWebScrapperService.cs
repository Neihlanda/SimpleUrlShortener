using System;
using SimpleUrlShortener.Models;

namespace SimpleUrlShortener.Services
{
	public interface IWebScrapperService
    {
		public HtmlHead RetreiveHtmlHead(Uri uri);
	}
}

