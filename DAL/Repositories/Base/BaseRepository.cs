using DAL.EfStructures;
using DAL.Exceptions;
using Domain.Models.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DAL.Repositories.Base
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel, new()
    {
        private bool _isDisposed;
        private readonly bool _disposeContext;
        public ApplicationDbContext Context { get; }
        public DbSet<T> Table { get; }

        protected BaseRepository(ApplicationDbContext context)
        {
            Context = context;
            _disposeContext = false;
            Table = Context.Set<T>();
        }

        protected BaseRepository(DbContextOptions<ApplicationDbContext> options) : this(new ApplicationDbContext(options))
        {
            _disposeContext = true;
        }


        public virtual int Add(T entity, bool persist = true)
        {
            Table.Add(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual async Task<int> AddAsync(T entity, bool persist = true)
        {
            Table.Add(entity);

            return persist ? await SaveChangesAsync() : 0;
        }

        public virtual int AddRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.AddRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public void ExecuteQuery(string sql, object[] sqlParametersObjects)
            => Context.Database.ExecuteSqlRaw(sql, sqlParametersObjects);

        public virtual IEnumerable<T> FindAll() => Table;

        public virtual IEnumerable<T> FindAllIgnoreQueryFilters() => Table.IgnoreQueryFilters();

        public virtual T? FindOneById(int? id) => Table.Find(id);

        public virtual async Task<T?> FindOneByNameAsync(string name)
            => await Table.FirstOrDefaultAsync(e => e.Name == name);

        public virtual T? FindOneByNameAsNoTracking(string name)
            => Table.AsNoTrackingWithIdentityResolution().FirstOrDefault(x => x.Name == name);

        public T? FindOneByNameIgnoreQueryFilters(string name)
            => Table.IgnoreQueryFilters().FirstOrDefault(x => x.Name == name);

        public virtual async Task<int> UpdateAsync(T entity, bool persist = true)
        {
            Table.Update(entity);

            return persist ? await SaveChangesAsync() : 0;
        }

        public virtual int Update(T entity, bool persist = true)
        {
            Table.Update(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual int UpdateRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.UpdateRange(entities);
            return persist ? SaveChanges() : 0;
        }
        
        public virtual int Remove(T entity, bool persist = true)
        {
            Table.Remove(entity);
            return persist ? SaveChanges() : 0;
        }

        public virtual async Task<int> RemoveAsync(T entity, bool persist = true)
        {
            Table.Remove(entity);

            return persist ? await SaveChangesAsync() : 0;
        }

        public virtual int RemoveRange(IEnumerable<T> entities, bool persist = true)
        {
            Table.RemoveRange(entities);
            return persist ? SaveChanges() : 0;
        }

        public void SetEntryStateAdded(T entity) => Context.Entry(entity).State = EntityState.Added;

        public void SetEntryStateDeleted(T entity) => Context.Entry(entity).State = EntityState.Deleted;


        public int SaveChanges()
        {
            try
            {
                return Context.SaveChanges();
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CustomException("An error occurred during database update", e);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                return await Context.SaveChangesAsync();
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new CustomException("An error occurred during database update", e);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                if (_disposeContext)
                {
                    Context.Dispose();
                }
            }

            _isDisposed = true;
        }

        ~BaseRepository()
        {
            Dispose(false);
        }
    }
}
