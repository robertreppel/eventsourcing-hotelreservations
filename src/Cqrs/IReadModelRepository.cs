using System;
using System.Collections.Generic;

namespace Cqrs
{
    public interface IReadModelRepository<TModel>
    {
        IEnumerable<TModel> GetAll();
        TModel GetById(object id);
        IEnumerable<TModel> FindWhere(Func<TModel,bool> predicate);
        TModel FindOne(Func<TModel, bool> predicate, bool throwIfNotFound = true);

        void Save(TModel model);
        void Delete(object id);
    }
}