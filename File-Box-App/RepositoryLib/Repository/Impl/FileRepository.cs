using RepositoryLib.Models;
using System.Linq.Expressions;

namespace RepositoryLib.Repository.Impl
{
    public class FileRepository : IFileRepository
    {
        private readonly FileBoxDbContext m_dbContext;

        public FileRepository(FileBoxDbContext dbContext)
        {
            m_dbContext = dbContext;
        }

        public IEnumerable<FileboxFile> All => throw new NotImplementedException();

        public long Count()
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }

        public void Delete(FileboxFile t)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(FileboxFile t)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(long id)
        {
            throw new NotImplementedException();
        }

        public void DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FileboxFile>> FindAllAsync()
        {
            return Task.Run(() => m_dbContext.FileboxFiles).Result;
        }

        public IEnumerable<FileboxFile> FindByFilter(Expression<Func<FileboxFile, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFile>> FindByFilterAsync(Expression<Func<FileboxFile, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public FileboxFile FindById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxFile> FindByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFile> FindByIds(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFile>> FindByIdsAsync(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public FileboxFile Save(FileboxFile t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFile> Save(IEnumerable<FileboxFile> entities)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxFile> SaveAsync(FileboxFile t)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFile>> SaveAsync(IEnumerable<FileboxFile> entities)
        {
            throw new NotImplementedException();
        }
    }
}
