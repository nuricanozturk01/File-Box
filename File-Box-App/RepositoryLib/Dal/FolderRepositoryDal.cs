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

        private readonly IFolderRepository m_folderRepository;

        public FolderRepositoryDal(IFolderRepository folderRepository)
        {
            m_folderRepository = folderRepository;
        }


        public IEnumerable<FileboxFolder> AllFolders => m_folderRepository.All;

        public long Count()
        {
            return m_folderRepository.Count();
        }

        public async Task<long> CountAsync()
        {
            return await m_folderRepository.CountAsync();
        }

        public void Delete(FileboxFolder t)
        {
            m_folderRepository.Delete(t);
        }


        public void DeleteById(long id)
        {
            m_folderRepository.DeleteById(id);
        }


        public bool ExistsById(long id)
        {
            return m_folderRepository.ExistsById(id);
        }

        public async Task<bool> ExistsByIdAsync(long id)
        {
            return await m_folderRepository.ExistsByIdAsync(id);
        }

        public async Task<IEnumerable<FileboxFolder>> FindAllAsync()
        {
            return await m_folderRepository.FindAllAsync();
        }

        public IEnumerable<FileboxFolder> FindByFilter(Expression<Func<FileboxFolder, bool>> predicate)
        {
            return m_folderRepository.FindByFilter(predicate);
        }

        public async Task<IEnumerable<FileboxFolder>> FindByFilterAsync(Expression<Func<FileboxFolder, bool>> predicate)
        {
            return await m_folderRepository.FindByFilterAsync(predicate);
        }

        public FileboxFolder FindById(long id)
        {
            return m_folderRepository.FindById(id);
        }

        public async Task<FileboxFolder> FindByIdAsync(long id)
        {
            return await m_folderRepository.FindByIdAsync(id);
        }

        public IEnumerable<FileboxFolder> FindByIds(IEnumerable<long> ids)
        {
            return m_folderRepository.FindByIds(ids);
        }

        public async Task<IEnumerable<FileboxFolder>> FindByIdsAsync(IEnumerable<long> ids)
        {
            return await m_folderRepository.FindByIdsAsync(ids);
        }


        public FileboxFolder Save(FileboxFolder t)
        {
            return m_folderRepository.Save(t);
        }



        public async Task<FileboxFolder> SaveAsync(FileboxFolder t)
        {
           return await m_folderRepository.SaveAsync(t);
        }
        
        public void RemoveAll(IEnumerable<FileboxFolder> folders)
        {
            m_folderRepository.RemoveAllAsync(folders);
        }
        public FileboxFolder Update(FileboxFolder folder)
        {
            return m_folderRepository.Update(folder);
        }

    }
}
