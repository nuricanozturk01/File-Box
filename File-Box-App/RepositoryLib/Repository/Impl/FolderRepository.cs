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

        public IEnumerable<FileboxFolder> All => m_dbContext.FileboxFolders;

        public long Count()
        {
            return m_dbContext.FileboxFolders.Count();
        }

        public async Task<long> CountAsync()
        {
            return await m_dbContext.FileboxFolders.CountAsync();
        }

        public void Delete(FileboxFolder t)
        {
            m_dbContext.FileboxFolders.Remove(t);
            m_dbContext.SaveChanges();
        }

        public void DeleteAsync(FileboxFolder t)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(long id)
        {
            var folder = m_dbContext.FileboxFolders.Where(folder => folder.FolderId == id).FirstOrDefault();

            if (folder != null)
                Delete(folder);
        }

        public void DeleteByIdAsync(long id)
        {
            throw new NotImplementedException();
        }

        public bool ExistsById(long id)
        {
            return m_dbContext.FileboxFolders.Where(folder => folder.FolderId == id) != null;
        }

        public async Task<bool> ExistsByIdAsync(long id)
        {
            return await Task.Run(() => m_dbContext.FileboxFolders.FirstOrDefaultAsync(folder => folder.FolderId!= id).Result) != null;
        }

        public async Task<IEnumerable<FileboxFolder>> FindAllAsync()
        {
            return await m_dbContext.FileboxFolders.ToListAsync();
        }

        public IEnumerable<FileboxFolder> FindByFilter(Expression<Func<FileboxFolder, bool>> predicate)
        {
            return m_dbContext.FileboxFolders.Where(predicate);
        }

        public async Task<IEnumerable<FileboxFolder>> FindByFilterAsync(Expression<Func<FileboxFolder, bool>> predicate)
        {
            return await m_dbContext.FileboxFolders.Where(predicate).ToListAsync();
        }

        public FileboxFolder FindById(long id)
        {
            return m_dbContext.FileboxFolders.FirstOrDefault(folder => folder.FolderId == id);
        }

        public async Task<FileboxFolder> FindByIdAsync(long id)
        {
            return await m_dbContext.FileboxFolders.FirstOrDefaultAsync(folder => folder.FolderId == id);
        }

        public IEnumerable<FileboxFolder> FindByIds(IEnumerable<long> ids)
        {
            return m_dbContext.FileboxFolders.Where(folder => ids.Contains(folder.FolderId)).ToList();
        }

        public async Task<IEnumerable<FileboxFolder>> FindByIdsAsync(IEnumerable<long> ids)
        {
            return await m_dbContext.FileboxFolders.Where(folder => ids.Contains(folder.FolderId)).ToListAsync();
        }

        public void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath)
        {
            throw new NotImplementedException();
        }

        public void RemoveAllAsync(IEnumerable<FileboxFolder> folders)
        {
            m_dbContext.RemoveRange(folders);
            m_dbContext.SaveChanges();
        }

        public FileboxFolder Save(FileboxFolder t)
        {
            var folder = m_dbContext.FileboxFolders.Add(t).Entity;
            m_dbContext.SaveChanges();
            return folder;
        }

        public IEnumerable<FileboxFolder> Save(IEnumerable<FileboxFolder> entities)
        {
            throw new NotImplementedException();
        }

        public async Task<FileboxFolder> SaveAsync(FileboxFolder t)
        {
            var entry = m_dbContext.FileboxFolders.Add(t);
            await m_dbContext.SaveChangesAsync();
            return entry.Entity;
        }

        public async Task<IEnumerable<FileboxFolder>> SaveAsync(IEnumerable<FileboxFolder> entities)
        {
            entities.ToList().ForEach(f => m_dbContext.FileboxFolders.Add(f));
            await m_dbContext.SaveChangesAsync();
            return entities;
        }

        public FileboxFolder Update(FileboxFolder folder)
        {
            m_dbContext.Update(folder);
            m_dbContext.SaveChanges();
            return folder;
        }

        public async Task UpdateAll(IEnumerable<FileboxFolder> folders)
        {
            //await Task.Run(() => folders.ToList().ForEach(folder => m_dbContext.Update(folder)));
            await m_dbContext.SaveChangesAsync();
        }
    }
}
