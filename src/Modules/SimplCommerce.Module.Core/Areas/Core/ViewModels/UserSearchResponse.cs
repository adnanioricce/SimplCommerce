using System;
using System.Collections.Generic;

namespace SimplCommerce.Module.Core.Areas.Core.ViewModels
{
    public class UserSearchResponse
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> CustomerGroups { get; set; } = new List<string>();
        public DateTimeOffset CreatedOn { get; set; }
    }
}
