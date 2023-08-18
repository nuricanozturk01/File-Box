using RepositoryLib.DTO;

namespace Service.Services.FileServicePath
{
    public interface IFileService
    {






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
        Task<string> DeleteFile(long folderId, Guid guid);






        /*
         * 
         * 
         * Rename file and update its filepath and updated date informations with given fileId and new File Name.
         * 
         * 
         */
        Task<FileViewDto> RenameFile(long fileId, string newFileName, Guid userId);






        /*
         * 
         * Get all files from db with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        Task<IEnumerable<FileViewDto>> GetFilesByFolderIdAsync(long folderId, Guid userId, string currentToken);





        /*
         * 
         * Get all files from db with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        Task<IEnumerable<FileViewDto>> GetFilesByFileExtensionAndFolderIdAsync(long folderId, string? fileExtension, Guid userId);





        /*
         * 
         * Sort files with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        Task<IEnumerable<FileViewDto>> SortFilesByFileBytesAsync(long folderId, Guid userId);




        /*
         * 
         * Sort files with given folder ıd and user ıd about creation date
         * 
         * 
         */
        Task<IEnumerable<FileViewDto>> SortFilesByCreationDateAsync(long folderId, Guid userId);




        /*
         * 
         * Find File with given parameter is file id and user id 
         * 
         */
        Task<FileViewDto> FindFileByFileId(long fileId, Guid userId);
    }
}
