using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using Scorecraft.Entities;

namespace Scorecraft.Data
{
    public class Repository<TModel> : IRepository<TModel>
        where TModel : class
    {
        public DbSet<TModel> Data { get; }

        protected DbContext Context { get; }

        public Repository(DbContext context)
        {
            Context = context;
            Data = Context.Set<TModel>();
        }

        private IQueryable<TModel> BuildQuery(Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            Func<IQueryable<TModel>, IQueryable<TModel>> including = null, bool asNoTracking = true)
        {
            var query = asNoTracking ? Data.AsNoTracking() : Data.AsQueryable();

            if (including != null)
            {
                query = including(query);
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }

            return filter == null ? query : query.Where(filter);
        }

        public virtual bool Any(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IQueryable<TModel>> including = null)
        {
            return BuildQuery(filter, null, including).Any();
        }

        public virtual int Count(Expression<Func<TModel, bool>> filter = null, Func<IQueryable<TModel>, IQueryable<TModel>> including = null)
        {
            return BuildQuery(filter, null, including).Count();
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        public virtual void Create(TModel entity)
        {
            Data.Add(entity);
        }

        public virtual void CreateAll(IEnumerable<TModel> entities)
        {
            Data.AddRange(entities);
        }

        public virtual void Delete(TModel entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                Data.Attach(entity);
            }
            Data.Remove(entity);
        }

        public virtual void DeleteAll(IEnumerable<TModel> entities)
        {
            Data.RemoveRange(entities);
        }

        public virtual void Update(TModel entity)
        {
            Data.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateAll(IEnumerable<TModel> entities)
        {
            Data.AddOrUpdate(entities.ToArray());
        }

        public virtual TModel Select(params object[] keys)
        {
            return Data.Find(keys);
        }

        public virtual IEnumerable<TModel> Search(Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            Func<IQueryable<TModel>, IQueryable<TModel>> including = null, bool asNoTracking = true)
        {
            return BuildQuery(filter, orderBy, including, asNoTracking).ToList();
        }

        public virtual Pagination<TModel> Paginate(int pageIndex = 0, int pageSize = 15, Expression<Func<TModel, bool>> filter = null,
            Func<IQueryable<TModel>, IOrderedQueryable<TModel>> orderBy = null,
            Func<IQueryable<TModel>, IQueryable<TModel>> including = null)
        {
            var query = BuildQuery(filter, orderBy, including);
            Pagination<TModel> list = new Pagination<TModel>(pageIndex, pageSize, query.Count());

            list.AddRange(query.Skip(pageIndex * pageSize).Take(pageSize).ToList());
            return list;
        }
    }

    public class Repository<TEntity, TKey> : Repository<TEntity>, IRepository<TEntity, TKey>, IRepository<TEntity>
        where TEntity : class, IEntity<TKey>
    {
        public Repository(DbContext context)
            : base(context)
        { }

        public virtual TEntity Select(TKey id)
        {
            return base.Select(id);
        }
    }
}