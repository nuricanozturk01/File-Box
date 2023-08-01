using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IFolderRepository : ICrudRepository<FileboxFolder, long>
    {





        /*
         * 
         * Remove All Fodlers with given IEnumerable<FileBoxFolder> parameter 
         * 
         * 
         */
        void RemoveAllAsync(IEnumerable<FileboxFolder> folder);





        /*
         * 
         * Remove All Folders with given FileBoxFolder parameter 
         * 
         * 
         */
        FileboxFolder Update(FileboxFolder folder);





        /*
         * 
         * Update All Folders with given IEnumerable<FileBoxFolder> parameter 
         * 
         * 
         */
        Task UpdateAll(IEnumerable<FileboxFolder> folder);
    }
}
