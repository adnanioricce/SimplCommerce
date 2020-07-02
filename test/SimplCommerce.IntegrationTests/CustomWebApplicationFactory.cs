using System.IO;
namespace SimplCommerce.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
    {

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostingContext,configurationBuilder) => {
                var startupType = typeof(TStartup);
                var webAppPath = Directory.GetFullPath(GetWebApplicationPath("src/SimplCommerce.WebHost",Directory.GetCurrentDirectory()));
                configurationBuilder.AddJsonFile($"{webAppPath}/appsettings.json",optional:true,reloadOnChange:true);
                configurationBuilder.AddEnvironmentVariables();
            });
            builder.ConfigureServices(services => {
                var provider = new ServiceCollection()
                    .AddEntityFramework()
                    .BuildServiceProvider();
                services.AddDbContext<SimplDbContext>(options => {
                    options.UseSqlite("DataSource=:memory:");
                    options.UseInternalServiceProvider(provider);
                });
            });                        
            // builder.
        }        
        private string GetWebApplicationPath(string target,string current){
            string nextPath = Path.Combine(current, target);
            if(Directory.Exists(nextPath)){
               return nextPath; 
            }
            return GetWebApplicationPath(Directory.GetParent(current).FullPath,current);
        }
    }
}