using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;

namespace Scorecraft.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        public DbContext Context { get; }

        public UnitOfWork(DbContext context)
        {
            Context = context;
        }

        public void Commit()
        {
            Context.SaveChanges();
        }

        protected virtual IEnumerable<ObjectParameter> GetParameters(object obj)
        {
            List<ObjectParameter> result = new List<ObjectParameter>();
            if (obj == null) return result;

            var props = obj.GetType().GetProperties();
            foreach (PropertyInfo p in props)
            {
                if (p.GetCustomAttributes<NotMappedAttribute>().Any()) continue;

                ObjectParameter prm = new ObjectParameter(p.Name, p.PropertyType)
                {
                    Value = p.GetValue(obj) ?? DBNull.Value
                };

                result.Add(prm);
            }

            return result;
        }

        public virtual int Execute(string query, object value = null, object output = null)
        {
            var parameters = GetParameters(value).ToList();
            var outparams = GetParameters(output).ToList();
            parameters.AddRange(outparams);

            int result = Context.Database.ExecuteSqlCommand(query, parameters);
            foreach (var param in outparams)
            {
                output.GetType().GetProperty(param.Name).SetValue(output, param.Value);
            }
            return result;
        }

        public virtual IEnumerable<TModel> Query<TModel>(string query, object value = null, object output = null)
            where TModel : class
        {
            var parameters = GetParameters(value).ToList();
            var outparams = GetParameters(output).ToList();
            parameters.AddRange(outparams);

            var result = Context.Database.SqlQuery<TModel>(query, parameters).ToList();
            foreach (var param in outparams)
            {
                output.GetType().GetProperty(param.Name).SetValue(output, param.Value);
            }

            return result;
        }
    }
}
