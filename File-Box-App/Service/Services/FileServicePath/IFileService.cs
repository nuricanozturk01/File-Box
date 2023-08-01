using RepositoryLib.DTO;

namespace Service.Services.FileServicePath
{
    public interface IFileService
    {





        /*
         * 
         * 
         *  Return the all files on specific folder with given parameters userId and folderId
         * 
         * 
         */
        Task<IEnumerable<FileViewDto>?> GetFilesByUserIdAndFolderIdAsync(Guid userId, long folderId);






        /*
         * 
         * 
         * Create Empty file with given FileSaveDto parameter.
         * 
         * 
         */
        Task<bool> CreateFile(FileSaveDto fileSaveDto);






        /*
         * 
         * 
         * Remove file with given file Id
         * 
         * 
         */
        Task<bool> DeleteFile(long folderId);






        /*
         * 
         * 
         * Rename file and update its filepath and updated date informations with given fileId and new File Name.
         * 
         * 
         */
        void RenameFile(long fileId, string newFileName);
    }
}
