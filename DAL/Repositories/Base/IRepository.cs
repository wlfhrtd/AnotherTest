using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Base
{
    public interface IRepository<T> : IDisposable
    {
        int Add(T entity, bool persist = true);
        int AddRange(IEnumerable<T> entities, bool persist = true);
        int Update(T entity, bool persist = true);
        int UpdateRange(IEnumerable<T> entities, bool persist = true);
        int Remove(T entity, bool persist = true);
        int RemoveRange(IEnumerable<T> entities, bool persist = true);
        T? FindOneById(int? id);
        T? FindOneByNameAsNoTracking(string name);
        T? FindOneByNameIgnoreQueryFilters(string name);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindAllIgnoreQueryFilters();
        void ExecuteQuery(string sql, object[] sqlParametersObjects);
        int SaveChanges();
    }
}
