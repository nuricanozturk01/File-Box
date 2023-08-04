using Microsoft.EntityFrameworkCore;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using System.Linq.Expressions;

namespace RepositoryLib.Dal
{
   /*
    * 
    * Helper class for user repository. This class is a facade pattern for repositories 
    * 
    */

    public class FolderRepositoryDal
    {

        private readonly IGenericRepository<FileboxFolder, long> m_folderRepository;
        
        public FolderRepositoryDal(IGenericRepository<FileboxFolder, long> folderRepository)
        {
            m_folderRepository = folderRepository;
        }






        public void Delete(FileboxFolder t)
        {
            m_folderRepository.Delete(t);
        }






        public async Task<IEnumerable<FileboxFolder>> FindAllAsync()
        {
            return await m_folderRepository.FindAllAsync();
        }






        public async Task<IEnumerable<FileboxFolder>> FindByFilterAsync(Expression<Func<FileboxFolder, bool>> predicate)
        {
            return await m_folderRepository.FindByFilterAsync(predicate);
        }






        public async Task<FileboxFolder> FindByIdAsync(long id)
        {
            return await m_folderRepository.FindByIdAsync(id);
        }






        public async Task<IEnumerable<FileboxFolder>> FindByIdsAsync(IEnumerable<long> ids)
        {
            return await m_folderRepository.FindByFilterAsync(f => ids.Contains(f.FolderId));
        }






        public async Task<FileboxFolder> Save(FileboxFolder t)
        {
            return await m_folderRepository.SaveAsync(t);
        }





        public async Task UpdateAll(IEnumerable<FileboxFolder> folders)
        {
            await m_folderRepository.UpdateAll(folders);
        }






        public void RemoveAll(IEnumerable<FileboxFolder> folders)
        {
            m_folderRepository.RemoveAllAsync(folders);

        }





        public FileboxFolder Update(FileboxFolder folder)
        {
            return m_folderRepository.Update(folder);
        }






        public void SaveChanges()
        {
            m_folderRepository.SaveChanges();
        }






        public async Task SaveChangesAsync()
        {
            await m_folderRepository.SaveChangesAsync();
        }






        public async Task<IEnumerable<FileboxFolder>> FindFoldersByUserId(Guid guid)
        {
            return await FindByFilterAsync(f => f.UserId == guid);
        }
    }
}
