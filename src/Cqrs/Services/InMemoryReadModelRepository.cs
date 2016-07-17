using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cqrs.Services
{
    public class InMemoryReadModelRepository<TModel> : IReadModelRepository<TModel>
    {
        private readonly ConcurrentDictionary<object, TModel> _store;
        private readonly Func<TModel, object> _getId;
        
        public InMemoryReadModelRepository(ConcurrentDictionary<object, TModel> store, Func<TModel, object> getId)
        {
            _store = store;
            _getId = getId;
        }

        public IEnumerable<TModel> GetAll()
        {
            return _store.Values;
        }

        public TModel GetById(object id)
        {
            TModel wot;
            if (!_store.TryGetValue(id, out wot))
            {
                throw new ArgumentException("Invalid id.");
            }
            return wot;
        }

        public IEnumerable<TModel> FindWhere(Func<TModel, bool> predicate)
        {
            return _store.Values.Where(predicate);
        }

        public TModel FindOne(Func<TModel, bool> predicate, bool throwIfNotFound = true)
        {
            var result = _store.Values.Where(predicate).SingleOrDefault();
            if (result == null && throwIfNotFound) throw new Exception("No results matching predicate.");
            return result;
        }

        public void Save(TModel wot)
        {
            _store.AddOrUpdate(_getId(wot), wot, (k, v) => wot);
        }

        public void Delete(object id)
        {
            TModel value;
            _store.TryRemove(id, out value);
        }
    }
}