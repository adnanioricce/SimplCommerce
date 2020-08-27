using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Extensions;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Models;
using SimplCommerce.WebHost;
using Xunit;

namespace SimplCommerce.IntegrationTests.Areas.Core.Controllers
{
    //TODO: need teardown and setup
    public class UserApiControllerTests : TestBase,IClassFixture<CustomWebApplicationFactory<Startup>>,IDisposable
    {
        private readonly WebApplicationFactory<Startup> _factory;                     
        public UserApiControllerTests(CustomWebApplicationFactory<Startup> factory) : base(factory)
        {
            _factory = factory;            
        }


        [Theory]
        [InlineData("email@sample.com", "", 1, 1, "01/06/2012")]
        [InlineData("", "", 1, 1, "01/06/2012")]
        public async Task POST_List_receives_SmartTableParam_Return_200_status_code_if_Valid(string email, string fullname, int roleId, int customerGroupId, string createOn)
        {
            var client = _factory.CreateClient();
            var baseCustomerGroup = BaseCustomerGroup();            
            var user = new User
            {
                Email = email,
                //I am curious to know what is the result of this...
                FullName = fullname,
                Roles = new []
                     {
                        new UserRole
                        {
                            RoleId = roleId
                        }
                     },
                CreatedOn = DateTimeOffset.ParseExact(createOn, "dd/MM/yyyy", CultureInfo.InvariantCulture)
            };
            var customerGroupUser = BaseCustomerGroupUser(0, 0);
            customerGroupUser.CustomerGroup = baseCustomerGroup;
            customerGroupUser.User = user;
            user.CustomerGroups.Add(customerGroupUser);
            try
            {
                _dbContext.Add(user);
                _dbContext.SaveChanges();
            }catch(Exception ex)
            {
                //TODO:
            }
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
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(200,(int)response.StatusCode);            
        }
        [Fact]             
        public async Task GET_Get_receives_long_id_return_200ok_result_with_user_model()
        {
            //Arrange
            var client = _factory.CreateClient();
            var user = GetDefaultUser();
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            var urlTemplate = "api/users/{0}";
            var url = string.Format(urlTemplate,user.Id);

            //Act
            var response = await client.GetAsync(url);
            var jsonResult = await response.Content.ReadAsJsonAsync<UserForm>();                        
            //Assert
            Assert.Equal(200,(int)response.StatusCode);
            Assert.Equal(jsonResult.Id,user.Id);
        }
        [Fact]        
        public async Task GET_Get_receives_long_id_return_404NotFound_result_when_id_not_found()
        {
            // Arrange
            var id = 0;
            var client = _factory.CreateClient();            
            var urlTemplate = "api/users/{0}";
            var url = string.Format(urlTemplate, id);
            // Act
            var response = await client.GetAsync(url);
            // Assert
            Assert.Equal(404, (int)response.StatusCode);
        }
        [Fact]
        public async Task POST_post_receives_user_form_model_when_valid_model_insert_it_on_database()
        {
            // Arrange
            var client = _factory.CreateClient();
            var url = "api/users/";            
            var user = GetDefaultUser();
            var userForm = new UserForm
            {
                Email = user.Email,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                VendorId = user.VendorId,
                RoleIds = user.Roles.Select(r => r.RoleId).ToList(),
                Password = "Asdf1234@"
            };
            // Act
            var response = await client.PostAsJsonAsync(url, userForm);            
            // Assert
            Assert.Equal(201, (int)response.StatusCode);            
        }
        [Fact]
        public async Task Put_put_receives_id_and_user_form_model_when_user_exists_return_accepted_result()
        {
            // Arrange
            var userManager = _factory.Services.GetService<UserManager<User>>();            
            HttpClient client = _factory.CreateClient();
            var urlTemplate = "api/users/{0}";
            User user = GetDefaultUser();
            user.UserName = "FAKE_USER_NAME";
            await userManager.CreateAsync(user);            
            var url = string.Format(urlTemplate, user.Id);
            var userForm = new UserForm
            {
                Email = "UPDATED" + user.Email,
                FullName = "UPDATED" + user.FullName,                
                PhoneNumber = user.PhoneNumber,
                VendorId = user.VendorId,
                CustomerGroupIds = user.CustomerGroups.Select(c => c.CustomerGroupId).ToList(),
                RoleIds = user.Roles.Select(r => r.RoleId).ToList(),
            };
            // Act 
            var response = await client.PutAsJsonAsync(url, userForm);
            // Assert
            Assert.Equal(202, (int)response.StatusCode);
        }
        private User GetDefaultUser()
        {
            return GetUser("FAKE@EMAIL.COM", "FAKE USER FULLNAME", DateTimeOffset.UtcNow, BaseCustomerGroup(), BaseCustomerGroupUser(0,0), new UserRole { RoleId = 1 });
        }
        private User GetUser(string email,string fullname,DateTimeOffset createdOn, CustomerGroup customerGroup,CustomerGroupUser customerGroupUser,params UserRole[] roles)
        {
            var user = new User
            {
                Email = email,
                //I am curious to know what is the result of this...
                FullName = fullname,
                Roles = roles,
                CreatedOn = createdOn
            };            
            customerGroupUser.CustomerGroup = customerGroup;
            customerGroupUser.User = user;
            user.CustomerGroups.Add(customerGroupUser);
            return user;
        }
        private CustomerGroup BaseCustomerGroup()
        {
            return new CustomerGroup
            {
                Description = "FAKE GROUP DESCRIPTION",
                Name = "FAKE GROUP",
                IsActive = true                
            };
        }
        private CustomerGroupUser BaseCustomerGroupUser(int userId,int customerGroupId)
        {
            return new CustomerGroupUser
            {
                UserId = userId,
                CustomerGroupId = customerGroupId
            };
        }
    }
}
