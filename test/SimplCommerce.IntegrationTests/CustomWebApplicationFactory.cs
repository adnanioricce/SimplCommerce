using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimplCommerce.Infrastructure.Filters;
using SimplCommerce.WebHost;

namespace SimplCommerce.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var builder = Host.CreateDefaultBuilder()
                              .ConfigureWebHostDefaults(x =>
                              {
                                  x.UseStartup<TStartup>()
                                   .UseTestServer()
                                   .UseUrls("localhost:49208");
                                  x.ConfigureServices(services => services.AddSingleton<IAuthorizationHandler, AllowAnonymous>());
                              });
            return builder;
        }
    }
}
