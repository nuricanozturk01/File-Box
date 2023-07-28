using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepositoryLib.Models;
using System.Linq.Expressions;

namespace RepositoryLib.Repository.Impl
{
    public class FolderRepository : IFolderRepository
    {
        private readonly FileBoxDbContext m_dbContext;

        public FolderRepository(FileBoxDbContext dbContext)
        {
            m_dbContext = dbContext;
        }

        public IEnumerable<FileboxFolder> All => throw new NotImplementedException();

        public long Count()
        {
            throw new NotImplementedException();
        }

        public Task<long> CountAsync()
        {
            throw new NotImplementedException();
        }

        public void Delete(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public void DeleteAsync(FileboxFolder t)
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

        public Task<IEnumerable<FileboxFolder>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFolder> FindByFilter(Expression<Func<FileboxFolder, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> FindByFilterAsync(Expression<Func<FileboxFolder, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public FileboxFolder FindById(long id)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxFolder> FindByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFolder> FindByIds(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> FindByIdsAsync(IEnumerable<long> ids)
        {
            throw new NotImplementedException();
        }

        public void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath)
        {
            throw new NotImplementedException();
        }

        public FileboxFolder Save(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFolder> Save(IEnumerable<FileboxFolder> entities)
        {
            throw new NotImplementedException();
        }

        public Task<FileboxFolder> SaveAsync(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFolder>> SaveAsync(IEnumerable<FileboxFolder> entities)
        {
            throw new NotImplementedException();
        }
    }
}
