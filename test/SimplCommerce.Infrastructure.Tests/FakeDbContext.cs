using Microsoft.EntityFrameworkCore;
using SimplCommerce.Module.Core;
using SimplCommerce.Module.Core.Data;

namespace SimplCommerce.Infrastructure.Tests
{
    public class FakeDbContext : SimplDbContext
    {
        public FakeDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
