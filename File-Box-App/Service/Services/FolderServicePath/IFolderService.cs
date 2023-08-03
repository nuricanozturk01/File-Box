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
        Task<bool> CreateFolder(FolderSaveDTO folderSaveDto);






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
        Task<(string oldPath, string newPath)> RenameFolder(long folderId, string newFolderName, Guid userId);






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
         * 
         * View folder with files
         * 
         * 
         */
        Task<IEnumerable<FoldersWithFilesDto>> FindFolderWithFiles(Guid guid);
    }
}
