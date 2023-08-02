﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleUrlShortener.Tests.FunctionnalTests
{
    [Collection("IntegrationDatabase")]
    internal class UrlShortenerControllerTests : IClassFixture<SimpleShortenerWebApplicationFactory>
    {
        private readonly SimpleShortenerWebApplicationFactory _factory;

        public UrlShortenerControllerTests(SimpleShortenerWebApplicationFactory webFactory)
        {
            _factory = webFactory;
        }


    }
}