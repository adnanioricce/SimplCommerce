using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Infrastructure.Tests.Data
{
    public class FakeRepository<T> : IRepositoryWithTypedId<T, long> where T : IEntityWithTypedId<long>
    {
        private readonly Dictionary<long, T> _data = new Dictionary<long, T>();        
        private long _counter = 1;
        public FakeRepository()
        {            
        }
        public FakeRepository(IEnumerable<T> entities)
        {
            AddRange(entities);
        }
        public void Add(T entity)
        {
            _data.TryGetValue(entity.Id, out var _entity);
            if (!(_entity is null)) return;
            entity.SetId(_counter);
            _data.Add(entity.Id, entity);
            ++_counter;
        }

        public void AddRange(IEnumerable<T> entities)
        {
            foreach(var entity in entities)
            {
                Add(entity);
            }
        }

        public IDbContextTransaction BeginTransaction()
        {
            //At this level, is probably best to write a integration test
            throw new System.NotImplementedException();
        }

        public IQueryable<T> Query()
        {
            return _data.Select(e => e.Value).AsQueryable();
        }

        public void Remove(T entity)
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
