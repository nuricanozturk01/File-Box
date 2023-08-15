using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace Service.Services.FolderService
{
    public interface IFolderService
    {






        /*
         * 
         * 
         * Get Folders for specific user
         * 
         * 
         */
        Task<IEnumerable<FolderViewDto>> GetFoldersByUserIdAsync(Guid userId);
        
        
        
        
        
        
        /*
         * 
         * 
         * Create Empty folder with given FolderSaveDTO parameter.
         * 
         * 
         */
        Task<(string folderPath, long folderId)> CreateFolder(FolderSaveDTO folderSaveDto);






        /*
         * 
         * 
         * Remove folder with given folderId parameter
         * 
         * 
         */
        Task<string> DeleteFolder(long folderId, Guid userID);






        /*
         * 
         * 
         * Rename Folder and update file and folder paths its files, subfolders and their files.
         * 
         * 
         */
        Task<FolderViewDto> RenameFolder(long folderId, string newFolderName, Guid userId);






        /*
         * 
         * 
         * Find Root folder by user uuid
         * 
         * 
         */
        Task<FolderViewDto> FindRootFolder(Guid guid);






        /*
         * 
         * Find all Folder and files with given user id
         * 
         */
        Task<IEnumerable<FoldersWithFilesDto>> FindFolderWithFiles(Guid guid);






        /*
         * 
         * Find all Folder and files with given user id and folder id
         * 
         */
        Task<IEnumerable<FoldersWithFilesDto>> FindFolderWithFiles(Guid guid, long folderId);





        /*
         * 
         * Find Folder with folder id and user id
         * 
         */
        Task<FolderViewDto> FindFolderWithFolderId(Guid guid, long folderId);
    }
}
