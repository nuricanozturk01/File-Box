using System.Linq.Expressions;

namespace RepositoryLib.Repository
{
    public interface IGenericRepository<T, ID> where T : class
    {
        Task<IEnumerable<T>> FindAllAsync();






        Task<IEnumerable<T>> FindByFilterAsync(Expression<Func<T, bool>> predicate);






        Task<T> FindByPredicateAsync(Expression<Func<T, bool>> predicate);






        Task<T> FindByIdAsync(ID id);






        Task<bool> ExistsByIdAsync(ID id);






        Task<T> SaveAsync(T entity);






        Task<IEnumerable<T>> SaveAllAsync(IEnumerable<T> entities);






        Task DeleteByIdAsync(ID id);






        Task Delete(T obj);






        Task SaveChangesAsync();






        void SaveChanges();






        T Update(T entity);






        Task RemoveAllAsync(IEnumerable<T> entities);






        T FindByPredicate(Expression<Func<T, bool>> predicate);






        /*
         * 
         * Update All Folders with given IEnumerable<FileBoxFolder> parameter 
         * 
         * 
         */
        Task UpdateAll(IEnumerable<T> folder);
    }
}
