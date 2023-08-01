using RepositoryLib.DTO;
using System.IO.Compression;
/*
 * 
 *  
 * This class Represent the Download Single and Multiple Files and Folders
 * 
 * 
 */
namespace Service.Services.DownloadService
{
    public class DownloadService : IDownloadService
    {

        /*
         * 
         * Download Multiple files zipped.
         * 
         */
        public async Task<(byte[], string, string)> DownloadMultipleFile(List<FolderUploadDto> filePaths)
        {
            var desktopFolderPath = Path.Combine("C:\\Users\\hp\\Desktop");

            var zipFileName = DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss]") + "_downloaded_files.zip";

            var zipFilePath = Path.Combine(desktopFolderPath, zipFileName);

            using (var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
            {
                foreach (var file in filePaths)
                    if (File.Exists(file.sourceFilePath))
                        zip.CreateEntryFromFile(file.sourceFilePath, Path.GetFileName(file.sourceFilePath), CompressionLevel.Optimal);
            }
            return (await File.ReadAllBytesAsync(zipFilePath), "application/zip", zipFileName);
        }






        /*
         * 
         * Download Multiple folder zipped. Include subfolders
         * 
         */
        public async Task<(byte[] bytes, string content, string fileName)> DownloadMultipleFolder(List<FolderUploadDto> filePath)
        {
            using (var zipArchive = ZipFile.Open("C:\\Users\\hp\\Desktop\\deneme.zip", ZipArchiveMode.Create))
            {
                foreach (var folderPath in filePath)
                {
                    string folderName = new DirectoryInfo(folderPath.sourceFilePath).Name;

                    foreach (string file in Directory.GetFiles(folderPath.sourceFilePath, "*", SearchOption.AllDirectories))
                    {
                        string relativePath = file.Replace(folderPath + "\\", "");
                        zipArchive.CreateEntryFromFile(file, Path.Combine(folderName, relativePath));
                    }
                }
            }
            return (await File.ReadAllBytesAsync("C:\\Users\\hp\\Desktop\\deneme.zip"), "application/zip", "deneme.zip");
        }






        /*
         * 
         * Download Single file.
         * 
         */
        public async Task<(byte[], string, string)> DownloadSingleFile(string filePath)
        {
            return (await File.ReadAllBytesAsync(filePath), "application/octet-stream", Path.GetFileName(filePath));
        }







        /*
         * 
         * Download Single folder zipped. Include subfolders
         * 
         */
        public async Task<(byte[] bytes, string content, string fileName)> DownloadSingleFolder(string filePath)
        {
            var dir = new DirectoryInfo(filePath);

            if (!dir.Exists)
                return (null, null, null);

            ZipFile.CreateFromDirectory(filePath, "C:\\Users\\hp\\Desktop\\deneme.zip");

            return (await File.ReadAllBytesAsync("C:\\Users\\hp\\Desktop\\deneme.zip"), "application/zip", "deneme.zip");
        }
    }
}
