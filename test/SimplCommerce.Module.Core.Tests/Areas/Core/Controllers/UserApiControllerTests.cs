using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Tests;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Areas.Core.Controllers;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;
using SimplCommerce.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SimplCommerce.Module.Core.Tests.Areas.Core.Controllers
{
    public class UserApiControllerTests
    {        

        private UserApiController CreateUserApiController(IRepository<User> userRepository)
        {
            return new UserApiController(userRepository,null);
        }

        [Theory]
        [InlineData("sample@email.com","sample name")]
        [InlineData("", "sample name")]
        [InlineData("sample@email.com", "")]
        [InlineData("", "")]
        public async Task QuickSearch_receives_UserSearchOption_object_Expected_to_return_OkObjectResult_if_has_any_data_matching_search_object(string email,string name)
        {
            // Arrange
            var userRepository = new FakeUserRepository(new User[] {
                new User
                {
                    Email = email,
                    FullName = name
                }
            });
            var userApiController = this.CreateUserApiController(userRepository);
            UserSearchOption searchOption = new UserSearchOption
            {
                Email = email,
                Name = name
            };

            // Act
            var response = await userApiController.QuickSearch(searchOption);
            var result = response as OkObjectResult;            
            // Assert
            Assert.NotNull(result);            
            Assert.Equal(200, result.StatusCode);            
        }

        [Theory]
        [InlineData("email@sample.com", "",1,1,"01/06/2012")]
        [InlineData("", "", 1, 1, "01/06/2012")]
        public void List_receives_SmartTableParam_with_at_least_one_predicate_with_at_least_one_field_Expected_to_return_list_of_users_that_match_object(string email, string fullname, int roleId,int customerGroupId,string createOn)
        {
            // Arrange
            var data = Enumerable.Range(1, 15).Select(i =>
                 new User
                 {
                     Email = i + email,
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
                     CreatedOn = DateTimeOffset.Parse(createOn)
                 });            
            var userRepository = new FakeUserRepository();
            userRepository.AddRange(data);
            var userApiController = this.CreateUserApiController(userRepository);
            SmartTableParam param = new SmartTableParam { 
                Search = new Search
                {
                    PredicateObject = JObject.FromObject( new
                    {
                        Email = email,
                        FullName = fullname,
                        RoleId = roleId,
                        CustomerGroupId = customerGroupId,
                        CreatedOn = new
                        {
                            after = DateTimeOffset.Parse(createOn)
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
            var response = userApiController.List(param);
            //I can't really know if it work, because can't find a way to get the value of the result object
            var result = response as JsonResult;
            var smartResult = result.Value;                        
            // Assert
            Assert.NotNull(smartResult);            
        }

        [Fact]
        public async Task Get_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            userRepository.Add(new User());
            var userApiController = this.CreateUserApiController(userRepository);
            long id = 1;

            // Act
            var response = await userApiController.Get(id);
            var result = response as JsonResult;
            var user = result.Value as UserForm;
            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(1,user.Id);            
        }

        [Fact]
        public async Task Post_StateUnderTest_ExpectedBehavior()
        {
            // Arrange            
            var userApiController = this.CreateUserApiController(new FakeUserRepository());
            UserForm model = new UserForm { 

            };

            // Act
            var result = await userApiController.Post(model);

            // Assert
            Assert.True(false);            
        }

        [Fact]
        public async Task Put_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var userRepository = (new FakeUserRepository());
            var expectedUser = new User
            {
                FullName = "fake name"
            };
            userRepository.Add(expectedUser);
            var userApiController = this.CreateUserApiController(userRepository);
            long id = 1;
            UserForm model = new UserForm { 
                FullName = expectedUser.FullName
            };

            // Act
            var response = await userApiController.Put(id,model);
            var result = response as AcceptedResult;            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(202,result.StatusCode);            
        }

        [Fact]
        public async Task Delete_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var sampleUser = new User();
            userRepository.Add(sampleUser);
            var userApiController = this.CreateUserApiController(userRepository);
            long id = 1;

            // Act
            var result = await userApiController.Delete(id);
            var value = result as NoContentResult;            
            // Assert
            Assert.NotNull(value);            
        }
    }
}
