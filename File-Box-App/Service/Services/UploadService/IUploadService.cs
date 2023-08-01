using Microsoft.AspNetCore.Http;
using RepositoryLib.DTO;

namespace Service.Services.UploadService
{
    public interface IUploadService
    {







        /*
         * 
         * Upload single file
         * returns the boolean value that if success return true else return false
         * 
         */
        Task<bool> UploadSingleFile(IFormFile formFile, long folderId, Guid guid);






        /*
         * 
         * Upload multiple files with using compressing
         * returns the boolean value that if success return true else return false
         * 
         */
        Task<bool> UploadMultipleFiles(List<IFormFile> formFile, long folderId, Guid uid);






        /*
         * 
         * Upload multiple folders with using compressing. But using on single folder. list length is one
         * returns the boolean value that if success return true else return false
         * 
         */
        Task<bool> UploadMultipleFolder(List<FolderUploadDto> sourcePaths, long folderId, Guid uid);
    }
}
