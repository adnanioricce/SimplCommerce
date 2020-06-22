using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Models;
using SimplCommerce.Module.Core.Models;

namespace SimplCommerce.Infrastructure.Tests
{
    public class FakeUserRepository : IRepository<User>
    {
        private readonly Dictionary<long, User> _data = new Dictionary<long, User>();
        private long _counter = 1;
        public FakeUserRepository()
        {
        }
        public FakeUserRepository(IEnumerable<User> entities)
        {
            AddRange(entities);
        }
        public void Add(User entity)
        {
            _data.TryGetValue(entity.Id, out var _entity);
            if (!(_entity is null)) return;
            entity.SetId(_counter);
            _data.Add(entity.Id, entity);
            ++_counter;
        }       
        public void AddRange(IEnumerable<User> entities)
        {
            foreach (var entity in entities)
            {
                Add(entity);
            }
        }

        public IDbContextTransaction BeginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<User> Query()
        {
            return new TestAsyncEnumerable<User>(_data.Select(e => e.Value));
        }

        public void Remove(User entity)
        {
            _data.Remove(entity.Id);
        }

        public void SaveChanges()
        {
            return;
        }

        public Task SaveChangesAsync()
        {
            return Task.Delay(0);
        }
    }
}
