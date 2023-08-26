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

    public class FileRepositoryDal
    {
        private readonly IGenericRepository<FileboxFile, long> m_fileRepository;

        public FileRepositoryDal(IGenericRepository<FileboxFile, long> fileRepository)
        {
            m_fileRepository = fileRepository;
        }





        public async Task Delete(FileboxFile t)
        {
            await m_fileRepository.Delete(t);
        }






        public void DeleteById(long id)
        {
            m_fileRepository.DeleteByIdAsync(id);
        }






        public async Task<IEnumerable<FileboxFile>> FindAllAsync()
        {
            return await m_fileRepository.FindAllAsync();
        }






        public async Task<IEnumerable<FileboxFile>> FindByFilterAsync(Expression<Func<FileboxFile, bool>> predicate)
        {
            return await m_fileRepository.FindByFilterAsync(predicate);
        }






        public async Task<FileboxFile> FindById(long id)
        {
            return await m_fileRepository.FindByIdAsync(id);
        }


        public async Task RemoveAll(IEnumerable<FileboxFile> files)
        {
            await m_fileRepository.RemoveAll(files);
        }



        public async Task<FileboxFile> FindByIdAsync(long id)
        {
            return await m_fileRepository.FindByIdAsync(id);
        }






        public async Task<IEnumerable<FileboxFile>> FindByIdsAsync(IEnumerable<long> ids)
        {
            return await m_fileRepository.FindByFilterAsync(f => ids.Contains(f.FileId));
        }






        public async Task<IEnumerable<FileboxFile>> FindFilesByFolderId(long folderId)
        {
            return await FindByFilterAsync(f => f.FolderId == folderId);
        }






        public async Task<FileboxFile> Save(FileboxFile t)
        {
            var extension = string.IsNullOrEmpty(t.FileType) ? "N/A" : t.FileType;
            t.FileType = extension;

            return await m_fileRepository.SaveAsync(t);
        }






        public async Task<FileboxFile> SaveAsync(FileboxFile t)
        {
            var extension = string.IsNullOrEmpty(t.FileType) ? "N/A" : t.FileType;
            t.FileType = extension;

            return await m_fileRepository.SaveAsync(t);
        }






        public async Task SaveChangesAsync()
        {
            await m_fileRepository.SaveChangesAsync();
        }






        public FileboxFile Update(FileboxFile file)
        {
            return m_fileRepository.Update(file);
        }






        public async Task UpdateAll(IEnumerable<FileboxFile> files)
        {
            await m_fileRepository.UpdateAll(files);
        }

        public async Task<IEnumerable<FileboxFile>> FindFilesByFolderPath(string oldFolderPathStartsWithRoot)
        {
            return await m_fileRepository.FindByFilterAsync(f => f.FilePath == oldFolderPathStartsWithRoot);
        }
    }
}
