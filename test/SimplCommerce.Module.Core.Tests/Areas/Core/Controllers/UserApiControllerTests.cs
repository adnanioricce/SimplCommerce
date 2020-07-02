using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Tests.Data;
using SimplCommerce.Infrastructure.Tests.Mocks;
using SimplCommerce.Infrastructure.Web.SmartTable;
using SimplCommerce.Module.Core.Areas.Core.Controllers;
using SimplCommerce.Module.Core.Areas.Core.ViewModels;
using SimplCommerce.Module.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SimplCommerce.Module.Core.Tests.Areas.Core.Controllers
{
    public class UserApiControllerTests
    {        
           
        [Theory]
        [InlineData("sample@email.com","sample name")]
        [InlineData("", "sample name")]
        [InlineData("sample@email.com", "")]
        [InlineData("", "")]
        public async Task QuickSearch_receives_UserSearchOption_object_Expected_to_return_OkObjectResult_if_has_any_data_matching_search_object(string email,string name)
        {            
            var userRepository = new FakeUserRepository();
            var userApiController = new UserApiController(userRepository, null);
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
            var data = new User
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
                     CreatedOn = DateTimeOffset.ParseExact(createOn,"dd/MM/yyyy",CultureInfo.InvariantCulture)
                 };            
            var userRepository = new FakeUserRepository();
            userRepository.Add(data);
            var userApiController = new UserApiController(userRepository, null);
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
            var response = userApiController.List(param);
            //I can't really know if it work, because can't find a way to get the value of the result object
            var result = response as JsonResult;
            var smartResult = result.Value as SmartTableResult<UserSearchResponse>;                        
            // Assert
            Assert.NotNull(smartResult);
            Assert.NotEmpty(smartResult.Items);            
        }

        [Fact]
        public async Task Get_receives_user_id_Expected_return_user_with_specified_id()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var user = new User();
            long id = 1;
            user.SetId(id);
            userRepository.Add(user);
            var userApiController = new UserApiController(userRepository,null);
            

            // Act
            var response = await userApiController.Get(id);
            var result = response as JsonResult;
            var userResult = result.Value as UserForm;
            // Assert
            //Assert.Equal(200, result.StatusCode);
            Assert.Equal(user.Id,userResult.Id);            
        }
        [Fact]
        public async Task Get_receives_null_object_Expected_return_not_found_result()
        {
            // Arrange
            var userRepository = new FakeUserRepository();                        
            var userApiController = new UserApiController(userRepository, null);
            // Act
            var response = await userApiController.Get(0);
            var result = response as NotFoundResult;
            // Assert            
            Assert.Equal(404, result.StatusCode);
        }        
        [Fact]
        public async Task Post_receives_user_form_object_Expected_created_at_action_result()
        {
            // Arrange            
            var userApiController = new UserApiController(new FakeUserRepository(), MockHelpers.MockUserManager(new List<User>()).Object);
            UserForm model = new UserForm { 
                Id = 3,
                Email = $"{Guid.NewGuid().ToString()}@email.com",
                FullName = "full fake name",
                Password = "asdf1234",
                PhoneNumber = "1140028922",
                RoleIds = new List<long>() { (long)Roles.Customer },                
            };

            // Act
            var response = await userApiController.Post(model);
            var result = response as CreatedAtActionResult;            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);            
        }

        [Fact]
        public async Task Put_receives_id_and_updated_state_Expected_accepted_result_if_update_is_successful()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var expectedUser = new User
            {
                FullName = "fake name",
                Roles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = 1L                        
                    },
                    new UserRole
                    {
                        RoleId = 2L
                    }
                }
            };
            userRepository.Add(expectedUser);
            var userApiController = new UserApiController(userRepository, MockHelpers.MockUserManager(new List<User> { expectedUser }).Object);
            long id = 1;
            UserForm model = new UserForm { 
                FullName = expectedUser.FullName + " updated",
                RoleIds = new long[] { 1L }
            };

            // Act
            var response = await userApiController.Put(id,model);
            var result = response as AcceptedResult;            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(202,result.StatusCode);            
        }
        [Fact]
        public async Task Put_receives_id_with_no_user_associated_Expected_not_found_result()
        {
            // Arrange
            var userRepository = new FakeUserRepository();            
            var userApiController = new UserApiController(userRepository, MockHelpers.MockUserManager(new List<User> { }).Object);                        

            // Act
            var response = await userApiController.Put(0, null);
            var result = response as NotFoundResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
        [Fact]
        public async Task Delete_receives_user_id_Expected_no_content_result()
        {
            // Arrange
            var userRepository = new FakeUserRepository();
            var sampleUser = new User();
            userRepository.Add(sampleUser);
            var userApiController = new UserApiController(userRepository, null);
            long id = 1;

            // Act
            var response = await userApiController.Delete(id);
            var result = response as NoContentResult;            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }
        [Fact]
        public async Task Delete_receives_non_existing_user_id_Expected_not_found_result()
        {
            // Arrange
            var userRepository = new FakeUserRepository();                        
            var userApiController = new UserApiController(userRepository, null);            

            // Act
            var response = await userApiController.Delete(1);
            var result = response as NotFoundResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }
    }
}
