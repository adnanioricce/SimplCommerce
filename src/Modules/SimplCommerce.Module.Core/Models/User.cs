using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Module.Core.Models
{
    public class User : IdentityUser<long>, IEntityWithTypedId<long>, IExtendableObject
    {
        public User()
        {
            CreatedOn = DateTimeOffset.Now;
            LatestUpdatedOn = DateTimeOffset.Now;
        }        

        public const string SettingsDataKey = "Settings";

        public Guid UserGuid { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string FullName { get; set; }

        public long? VendorId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset LatestUpdatedOn { get; set; }

        public IList<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();

        public UserAddress DefaultShippingAddress { get; set; }

        public long? DefaultShippingAddressId { get; set; }

        public UserAddress DefaultBillingAddress { get; set; }

        public long? DefaultBillingAddressId { get; set; }

        [StringLength(450)]
        public string RefreshTokenHash { get; set; }

        public IList<UserRole> Roles { get; set; } = new List<UserRole>();

        public IList<CustomerGroupUser> CustomerGroups { get; set; } = new List<CustomerGroupUser>();

        [StringLength(450)]
        public string Culture { get; set; }

        /// <inheritdoc />
        public string ExtensionData { get; set; }

        public void SetId(long id)
        {
            Id = id;
        }
        public void AddRoles(IEnumerable<UserRole> roles)
        {
            foreach (var role in roles)
            {
                Roles.Add(role);
            }
        }    
        public void RemoveRoles(IEnumerable<long> roleIds)
        {
            var roles = this.Roles.Where(userRole => !roleIds.Contains(userRole.RoleId)).ToList();
            RemoveRoles(roles);
        }
        public void RemoveRoles(IEnumerable<UserRole> roles)
        {
            foreach (var deletedUserRole in roles)
            {
                deletedUserRole.User = null;
                this.Roles.Remove(deletedUserRole);
            }
        }
        public bool HasRole(long roleId)
        {
            return this.Roles.Any(u => u.RoleId == roleId);
        }
    }
}
