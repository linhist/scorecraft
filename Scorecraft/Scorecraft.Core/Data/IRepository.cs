using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Scorecraft.Entities;

namespace Scorecraft.Data
{
    public interface IRepository<TModel>
        where TModel : class
    {
        DbSet<TModel> Data { get; }

        bool Any(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IQueryable<TModel>> including = null);

        int Count(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IQueryable<TModel>> including = null);

        void Commit();

        void Create(TModel entity);

        void CreateAll(IEnumerable<TModel> entity);

        void Delete(TModel entity);

        void DeleteAll(IEnumerable<TModel> entities);

        void Update(TModel entity);

        void UpdateAll(IEnumerable<TModel> entities);

        TModel Select(params object[] keys);

        IEnumerable<TModel> Search(Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            Func<IQueryable<TModel>, IQueryable<TModel>> including = null, bool asNoTracking = true);

        Pagination<TModel> Paginate(int pageIndex = 0, int pageSize = 15, Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            Func<IQueryable<TModel>, IQueryable<TModel>> including = null);
    }

    public interface IRepository<TEntity, TKey> : IRepository<TEntity>
        where TEntity : class, IEntity<TKey>
    {
        TEntity Select(TKey id);
    }
}
