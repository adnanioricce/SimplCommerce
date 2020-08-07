using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Models;

namespace SimplCommerce.Infrastructure.Tests
{
    public class FakeRepository<T> : IRepository<T> where T : EntityBase
    {
        private Dictionary<long,T> _data = new Dictionary<long, T>();
        private long _counter = 1;
        public void Add(T entity)
        {
            _data.TryGetValue(entity.Id,out var value);
            if(!(value is null)) return;
            entity.SetId(_counter);
            _data.Add(entity.Id,entity);
            ++_counter;
        }

        public void AddRange(IEnumerable<T> entity)
        {
            throw new System.NotImplementedException();
        }

        public IDbContextTransaction BeginTransaction()
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<T> Query()
        {
            return _data.Select(d => d.Value).AsQueryable();
        }

        public void Remove(T entity)
        {
            throw new System.NotImplementedException();
        }

        public void SaveChanges()
        {
            //...
        }

        public Task SaveChangesAsync()
        {
            return Task.Delay(0);            
        }
    }
}
