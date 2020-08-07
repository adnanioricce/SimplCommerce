using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Extensions;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.WebHost;
using Xunit;

namespace SimplCommerce.IntegrationTests.Areas.Core.Controllers
{
    //TODO: need teardown and setup
    public class UserApiControllerTests : IClassFixture<CustomWebApplicationFactory<Startup>>,IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly DbContext _dbContext;
        private readonly string _dbName = "SimplCommerce";
        private readonly string currentTimeInHours = $"{DateTime.UtcNow.Hour}";
        private readonly string databaseSnapshotName = "";
        public UserApiControllerTests(CustomWebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<SimplDbContext>();
            databaseSnapshotName = $"{_dbName}_snapshot_{currentTimeInHours}";
            string snapshotScript = $@"CREATE DATABASE {_dbName}_snapshot_{currentTimeInHours} ON  
                                    ( NAME = {_dbName}, FILENAME =   
                                    '.\{_dbName}_snapshot_{currentTimeInHours}.ss' )  
                                    AS SNAPSHOT OF {_dbName};";
            dbContext.Database.ExecuteSqlRaw(snapshotScript);
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            _dbContext.Database.ExecuteSqlRaw($"RESTORE DATABASE {_dbName} FROM DATABASE_SNAPSHOT = {databaseSnapshotName}");
        }

        [Theory]
        [InlineData("email@sample.com", "", 1, 1, "01/06/2012")]
        [InlineData("", "", 1, 1, "01/06/2012")]
        public async Task POST_List_receives_SmartTableParam_Return_200_status_code_if_Valid(string email, string fullname, int roleId, int customerGroupId, string createOn)
        {
            var client = _factory.CreateClient();
            var user = new User
            {
                Email = email,
                //I am curious to know what is the result of this...
                FullName = fullname,
                Roles = new List<UserRole>
                     {
                        new UserRole
                        {
                            RoleId = roleId
                        }
                     },
                CustomerGroups = new List<CustomerGroupUser>
                     {
                         new CustomerGroupUser
                         {
                             CustomerGroupId = customerGroupId
                         }
                     },
                CreatedOn = DateTimeOffset.ParseExact(createOn, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            };
            var param = new SmartTableParam
            {
                Search = new Search
                {
                    PredicateObject = JObject.FromObject(new
                    {
                        Email = email,
                        FullName = fullname,
                        RoleId = roleId,
                        CustomerGroupId = customerGroupId,
                        CreatedOn = new
                        {
                            after = DateTimeOffset.ParseExact(createOn, "dd/MM/yyyy", CultureInfo.InvariantCulture)
                        }
                    })
                },
                Pagination = new Pagination
                {
                    Number = 1,
                },
                Sort = new Sort
                {
                    Predicate = ""
                }
            };
            // Act
            var response = await client.PostJsonAsync("api/users/grid",param);            
            Assert.Equal(200,(int)response.StatusCode);            
        }
    }
}
