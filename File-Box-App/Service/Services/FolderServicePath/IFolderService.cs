using RepositoryLib.DTO;

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
        Task<bool> DeleteFolder(long folderId);






        /*
         * 
         * 
         * Rename Folder and update file and folder paths its files, subfolders and their files.
         * 
         * 
         */
        void RenameFolder(long folderId, string newFolderName);
    }
}
