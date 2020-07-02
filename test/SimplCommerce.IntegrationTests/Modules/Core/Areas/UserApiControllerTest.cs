using System;
using SimplCommerce.WebHost;
namespace SimplCommerce.IntegrationTests.Modules.Core.Areas
{
    public class UserApiControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>,IDisposable
    {
        private HttpClient _client;
        private SimplDbContext _context;
        private IDbConnection _connection;
        public UserApiControllerTest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SimplDbContext>();
                _context = context;
                _connection = context.Database.GetDbConnection();
                _connection.Open();
                _context.Database.EnsureCreated();
            }
        }
        [Theory]
        [InLineData("username","fake@email.com")]
        public void GET_QuickSearch_From_name_or_email(string name,string email)
        {
            var searchOption = new UserSearchOption{
                Name = name,
                Email = email
            };
            _context.Add(new User{
                Name = name,
                Email = email
            });
            _context.SaveChanges();
            var response = _client.GetAsync(searchOption);
            var result = response as OkObjectResult;
            Assert.Equals(200,result.StatusCode);
        }
    }
}