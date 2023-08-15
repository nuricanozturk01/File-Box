using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RepositoryLib.Models;
using System;
using System.Linq.Expressions;

namespace RepositoryLib.Repository
{
    public class CrudRepository<T, ID> : IGenericRepository<T, ID> where T : class
    {
        private readonly FileBoxDbContext m_dbContext;
        private readonly DbSet<T> m_dbSet;

        public CrudRepository(FileBoxDbContext context)
        {
            m_dbContext = context ?? throw new ArgumentNullException(nameof(context));
            //m_dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            m_dbSet = m_dbContext.Set<T>();
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            return await m_dbSet.ToListAsync();
        }






        public async Task<IEnumerable<T>> FindByFilterAsync(Expression<Func<T, bool>> predicate)
        {
            using (var context = new FileBoxDbContext())
            {
                return await context.Set<T>().Where(predicate).AsNoTracking().ToListAsync();
            }
        }





        public async Task<T> FindByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            using (var context = new FileBoxDbContext())
            {
                return await context.Set<T>().FirstOrDefaultAsync(predicate);
            }
            
        }





        public async Task<T> FindById(object id)
        {

            return await m_dbSet.FindAsync(id);
        }






        public async Task<T> SaveAsync(T entity)
        {
            return (await m_dbContext.AddAsync(entity)).Entity;

        }






        public T Update(T entity)
        {
            using (var context = new FileBoxDbContext())
            {
                var entry = context.Update(entity);
                context.SaveChanges();
                return entry.Entity;
            }
          
        }






        public async Task DeleteByIdAsync(object id)
        {
            var entity = await m_dbSet.FindAsync(id);
            if (entity != null)
            {
                m_dbSet.Remove(entity);
            }
        }






        public async Task SaveChangesAsync()
        {
            await m_dbContext.SaveChangesAsync();
        }





        public void SaveChanges()
        {
            m_dbContext.SaveChanges();
        }






        public async Task<T> FindByIdAsync(ID id)
        {
            return await m_dbSet.FindAsync(id);
        }






        public async Task<bool> ExistsByIdAsync(ID id)
        {
            return await m_dbSet.FindAsync(id) != null;
        }






        public async Task<IEnumerable<T>> SaveAllAsync(IEnumerable<T> entities)
        {
            await m_dbSet.AddRangeAsync(entities);
            await SaveChangesAsync();
            return entities;
        }






        public async Task DeleteByIdAsync(ID id)
        {
            var obj = await m_dbSet.FindAsync(id);

            if (obj != null)
            {
                m_dbSet.Remove(obj);
                await SaveChangesAsync();
            }
        }

        public async Task Delete(T obj)
        {
            m_dbSet.Remove(obj);
            await SaveChangesAsync();
        }





        public async Task RemoveAll(IEnumerable<T> entities)
        {
            m_dbSet.RemoveRange(entities);
            await SaveChangesAsync();
        }








        public async Task UpdateAll(IEnumerable<T> folders)
        {
            m_dbSet.UpdateRange(folders);
            await SaveChangesAsync();
        }






        public T FindByPredicate(Expression<Func<T, bool>> predicate)
        {
            return m_dbSet.FirstOrDefault(predicate);
        }
    }
}
