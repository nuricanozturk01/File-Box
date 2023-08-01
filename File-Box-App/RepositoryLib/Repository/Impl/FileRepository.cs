using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<FileboxFile> All => m_dbContext.FileboxFiles;

        public long Count()
        {
            return m_dbContext.FileboxFiles.Count();
        }

        public async Task<long> CountAsync()
        {
            return await m_dbContext.FileboxFiles.CountAsync();
        }

        public void Delete(FileboxFile t)
        {
            m_dbContext.FileboxFiles.Remove(t);
            m_dbContext.SaveChanges();
        }

        public void DeleteById(long id)
        {
            var file = m_dbContext.FileboxFiles.Where(f => f.FileId == id).FirstOrDefault();

            if (file is not null)
                Delete(file);
        }

        public bool ExistsById(long id)
        {
            return m_dbContext.FileboxFiles.Where(f => f.FileId == id) != null;
        }

        public async Task<bool> ExistsByIdAsync(long id)
        {
            return await Task.Run(() => m_dbContext.FileboxFiles.FirstOrDefaultAsync(file => file.FileId != id).Result) != null;
        }

        public async Task<IEnumerable<FileboxFile>> FindAllAsync()
        {
            return Task.Run(() => m_dbContext.FileboxFiles).Result;
        }

        public IEnumerable<FileboxFile> FindByFilter(Expression<Func<FileboxFile, bool>> predicate)
        {
            return m_dbContext.FileboxFiles.Where(predicate);
        }

        public async Task<IEnumerable<FileboxFile>> FindByFilterAsync(Expression<Func<FileboxFile, bool>> predicate)
        {
            return await m_dbContext.FileboxFiles.Where(predicate).ToListAsync();
        }

        public FileboxFile FindById(long id)
        {
            return m_dbContext.FileboxFiles.FirstOrDefault(file => file.FileId == id);
        }

        public async Task<FileboxFile> FindByIdAsync(long id)
        {
            return await m_dbContext.FileboxFiles.FirstOrDefaultAsync(file => file.FileId == id);
        }

        public IEnumerable<FileboxFile> FindByIds(IEnumerable<long> ids)
        {
            return m_dbContext.FileboxFiles.Where(file => ids.Contains(file.FileId)).ToList();
        }

        public async Task<IEnumerable<FileboxFile>> FindByIdsAsync(IEnumerable<long> ids)
        {
            return await m_dbContext.FileboxFiles.Where(file => ids.Contains(file.FileId)).ToListAsync();
        }

        public void RemoveAllAsync(IEnumerable<FileboxFile> files)
        {
            m_dbContext.RemoveRange(files);
            m_dbContext.SaveChanges();
        }

        public FileboxFile Save(FileboxFile t)
        {
            var file = m_dbContext.FileboxFiles.Add(t).Entity;
            m_dbContext.SaveChanges();
            return file;
        }

       

        public async Task<FileboxFile> SaveAsync(FileboxFile t)
        {
            var entry = m_dbContext.FileboxFiles.Add(t);
            await m_dbContext.SaveChangesAsync();
            return entry.Entity;
        }

      

        public FileboxFile Update(FileboxFile file)
        {
            m_dbContext.Update(file);
            m_dbContext.SaveChanges();
            return file;
        }


        public async Task UpdateAll(IEnumerable<FileboxFile> files)
        {
            await m_dbContext.SaveChangesAsync();
        }
        //-----------------------------------------------------------------------------------------------------


        public Task<IEnumerable<FileboxFile>> SaveAsync(IEnumerable<FileboxFile> entities)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxFile> Save(IEnumerable<FileboxFile> entities)
        {
            throw new NotImplementedException();
        }


        public void DeleteAsync(FileboxFile t)
        {
            throw new NotImplementedException();
        }


        public void DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }
    }
}
