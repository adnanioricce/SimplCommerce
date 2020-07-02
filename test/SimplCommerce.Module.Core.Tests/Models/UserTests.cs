using SimplCommerce.Module.Core.Models;
using System.Collections.Generic;
using Xunit;

namespace SimplCommerce.Module.Core.Tests.Models
{    
    //?is this useful?
    public class UserTests
    {        
        [Fact]
        public void AddRoles_receives_user_role_collection_without_user_Expected_set_user_property_on_role_and_add_it_to_user_object()
        {
            // Arrange
            var user = new User();
            var roles = new List<UserRole> { 
                new UserRole
                {
                    RoleId = 1
                }
            };

            // Act
            user.AddRoles(roles);

            // Assert
            Assert.Equal(1,user.Roles.Count);
            Assert.Equal(user, roles[0].User);
        }

        [Fact]
        public void RemoveRoles_receives_role_ids_to_remove_Expected_remove_all_given_roles_with_id_from_collection_of_roles_in_user()
        {
            // Arrange
            var user = new User { 
                Roles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = 1
                    },
                    new UserRole
                    {
                        RoleId = 2
                    }
                }
            };
            var roleIds = new long[] {                
                2L
            };

            // Act
            user.RemoveRoles(roleIds);

            // Assert
            Assert.True(user.HasRole(2));            
        }

        [Fact]
        public void RemoveRoles_receives_collection_of_user_roles_Expected_remove_all_roles_from_user_with_roleid_equal_given_roles()
        {
            // Arrange
            var roles = new List<UserRole> {
                new UserRole
                    {
                        RoleId = 1
                    },
                new UserRole
                {
                    RoleId = 2
                }
            };
            var user = new User { 
                Roles = new List<UserRole>
                {
                    roles[1]
                }
            };
            

            // Act
            user.RemoveRoles(roles);

            // Assert
            Assert.True(!user.HasRole(roles[1].RoleId));            
        }

        [Fact]
        public void HasRole_receives_role_id_Expected_return_true_if_has_any_role_with_given_role_id()
        {
            // Arrange
            long roleId = 1;
            var user = new User
            {
                Roles = new List<UserRole>
                {
                    new UserRole
                    {
                        RoleId = roleId
                    }
                }
            };
            

            // Act
            var result = user.HasRole(roleId);

            // Assert
            Assert.True(result);            
        }
    }
}
