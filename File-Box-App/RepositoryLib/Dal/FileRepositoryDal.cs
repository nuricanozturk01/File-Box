using RepositoryLib.Models;
using RepositoryLib.Repository;
using System.Linq.Expressions;

namespace RepositoryLib.Dal
{
    public class FileRepositoryDal
    {
        private readonly IFileRepository m_fileRepository;

        public FileRepositoryDal(IFileRepository fileRepository)
        {
            m_fileRepository = fileRepository;
        }

        public IEnumerable<FileboxFile> All => m_fileRepository.All;

        public long Count()
        {
            return m_fileRepository.Count();
        }

        public async Task<long> CountAsync()
        {
            return await m_fileRepository.CountAsync();
        }

        public void Delete(FileboxFile t)
        {
            m_fileRepository.Delete(t);
        }

        public void DeleteById(long id)
        {
            m_fileRepository.DeleteById(id);
        }

        public bool ExistsById(long id)
        {
            return m_fileRepository.ExistsById(id);
        }

        public async Task<bool> ExistsByIdAsync(long id)
        {
            return await m_fileRepository.ExistsByIdAsync(id);
        }

        public async Task<IEnumerable<FileboxFile>> FindAllAsync()
        {
            return await m_fileRepository.FindAllAsync();
        }

        public IEnumerable<FileboxFile> FindByFilter(Expression<Func<FileboxFile, bool>> predicate)
        {
            return m_fileRepository.FindByFilter(predicate);
        }

        public async Task<IEnumerable<FileboxFile>> FindByFilterAsync(Expression<Func<FileboxFile, bool>> predicate)
        {
            return await m_fileRepository.FindByFilterAsync(predicate);
        }

        public FileboxFile FindById(long id)
        {
            return m_fileRepository.FindById(id);
        }

        public async Task<FileboxFile> FindByIdAsync(long id)
        {
            return await m_fileRepository.FindByIdAsync(id);
        }

        public IEnumerable<FileboxFile> FindByIds(IEnumerable<long> ids)
        {
            return m_fileRepository.FindByIds(ids);
        }

        public async Task<IEnumerable<FileboxFile>> FindByIdsAsync(IEnumerable<long> ids)
        {
            return await m_fileRepository.FindByIdsAsync(ids);
        }

        public void RemoveAllAsync(IEnumerable<FileboxFile> files)
        {
            m_fileRepository.RemoveAllAsync(files);
        }

        public FileboxFile Save(FileboxFile t)
        {
            return m_fileRepository.Save(t);
        }



        public async Task<FileboxFile> SaveAsync(FileboxFile t)
        {
            return await m_fileRepository.SaveAsync(t);
        }



        public FileboxFile Update(FileboxFile file)
        {
            return m_fileRepository.Update(file);
        }
    }
}
