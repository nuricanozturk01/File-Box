using RepositoryLib.DTO;

namespace Service.Services.DownloadService
{
    public interface IDownloadService
    {






        /*
         * 
         * Download Single File without zip feature.  
         * Given parameters are fileId and user uuid
         * Return the Triple structure and become the file on the controller.
         * 
         */
        Task<(byte[], string, string)> DownloadSingleFile(long fileId, Guid uid);






        /*
         * 
         * Download Multiple File with zip feature.  
         * Given parameters are List<MultipleFileDownloadDto> and user uuid
         * Return the byte of zip file and become the file on the controller.
         * 
         */
        Task<byte[]> DownloadMultipleFile(List<MultipleFileDownloadDto> filesDownloadDtoi, Guid uid);






        /*
         * 
         * Download Single Folder with zip feature. Include all subfolders and files. 
         * Given parameters are folderId and user uuid
         * Return the Triple structure and become the file on the controller.
         * 
         */
        Task<(byte[] bytes, string content, string fileName)> DownloadSingleFolder(long folderId, Guid uid);






        /*
         * 
         * Download Multiple File with zip feature. Include all subfolders and files. 
         * Given parameters are List<MultipleFolderDownloadDto> and user uuid
         * Return the Triple structure and become the file on the controller.
         * 
         */
        Task<(byte[] bytes, string content, string fileName)> DownloadMultipleFolder(List<MultipleFolderDownloadDto> folderDownloadDto, Guid uid);
    }
}
