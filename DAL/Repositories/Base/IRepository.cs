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
        Task<int> AddAsync(T entity, bool persist = true);
        int AddRange(IEnumerable<T> entities, bool persist = true);
        int Update(T entity, bool persist = true);
        Task<int> UpdateAsync(T entity, bool persist = true);
        int UpdateRange(IEnumerable<T> entities, bool persist = true);
        int Remove(T entity, bool persist = true);
        Task<int> RemoveAsync(T entity, bool persist = true);
        int RemoveRange(IEnumerable<T> entities, bool persist = true);
        T? FindOneById(int? id);
        Task<T?> FindOneByNameAsync(string name);
        T? FindOneByNameAsNoTracking(string name);
        T? FindOneByNameIgnoreQueryFilters(string name);
        IEnumerable<T> FindAll();
        IEnumerable<T> FindAllIgnoreQueryFilters();
        void ExecuteQuery(string sql, object[] sqlParametersObjects);
        void SetEntryStateAdded(T entity);
        void SetEntryStateDeleted(T entity);
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
