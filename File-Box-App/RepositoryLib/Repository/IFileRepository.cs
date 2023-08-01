using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IFileRepository : ICrudRepository<FileboxFile, long>
    {





        /*
         * 
         * Remove All Files with given IEnumerable<FileBoxFile> parameter 
         * 
         * 
         */
        void RemoveAllAsync(IEnumerable<FileboxFile> files);





        /*
         * 
         * Update File with given FileBoxFile parameter
         * 
         * 
         */
        FileboxFile Update(FileboxFile file);





        /*
         * 
         * Update All Files with given IEnumerable<FileboxFile> parameter 
         * 
         * 
         */
        Task UpdateAll(IEnumerable<FileboxFile> files);
    }
}
