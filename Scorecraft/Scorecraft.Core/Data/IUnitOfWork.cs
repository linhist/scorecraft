using System.Collections.Generic;
using System.Data.Entity;

namespace Scorecraft.Data
{
    public interface IUnitOfWork
    {
        DbContext Context { get; }

        void Commit();

        int Execute(string query, object value, object output);

        IEnumerable<TModel> Query<TModel>(string query, object value, object output)
            where TModel : class;
    }
}
