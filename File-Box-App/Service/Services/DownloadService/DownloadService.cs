using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
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
        private readonly FileRepositoryDal m_fileRepositoryDal;
        private readonly FolderRepositoryDal m_folderRepositoryDal;



        public DownloadService(FileRepositoryDal fileRepositoryDal, FolderRepositoryDal folderRepositoryDal)
        {
            m_fileRepositoryDal = fileRepositoryDal;
            m_folderRepositoryDal = folderRepositoryDal;
        }






        /*
         * 
         * Download Single File without zip feature.  
         * Given parameters are fileId and user uuid
         * Return the Triple structure and become the file on the controller.
         * 
         */
        public async Task<(byte[], string, string)> DownloadSingleFile(long fileId, Guid uid)
        {
            var file = await m_fileRepositoryDal.FindByIdAsync(fileId); // Find file given file id
            var folder = await m_folderRepositoryDal.FindByIdAsync(file.FolderId); // Find file's folder

            if (uid != folder.UserId) // control the user is valid
                return (null, null, null);

            var filePath = Util.DIRECTORY_BASE + file.FilePath; // real file path

            return (await File.ReadAllBytesAsync(filePath), "application/octet-stream", Path.GetFileName(file.FileName));
        }






        /*
         * 
         * Download Multiple File with zip feature.  
         * Given parameters are List<MultipleFileDownloadDto> and user uuid
         * Return the byte of zip file and become the file on the controller.
         * 
         */
        public async Task<byte[]> DownloadMultipleFile(List<MultipleFileDownloadDto> filesDownloadDto, Guid uid)
        {
            var listIds = filesDownloadDto.Select(dto => dto.fileId).ToList();
            var files = await m_fileRepositoryDal.FindByIdsAsync(listIds);

            // Control the owner of each file is same
            if (!await ControlFileOwnersAsync(files, uid))
                return null;

            using (var zip = new MemoryStream())
            {
                using (var archive = new ZipArchive(zip, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var filePath = Util.DIRECTORY_BASE + file.FilePath;

                        if (File.Exists(filePath))
                            archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath), CompressionLevel.Optimal);
                    }
                }
                return zip.ToArray();
            }
        }






        /*
        * 
        * Download Single Folder with zip feature. Include all subfolders and files. 
        * Given parameters are folderId and user uuid
        * Return the Triple structure and become the file on the controller.
        * 
        */
        public async Task<(byte[] bytes, string content, string fileName)> DownloadSingleFolder(long folderId, Guid uid)
        {
            var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

            if (uid != folder.UserId)
                return (null, null, null);

            var folderPath = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath);

            var dir = new DirectoryInfo(folderPath);

            var zipFileName = DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss]") + "_downloaded_files.zip";

            if (!dir.Exists)
                return (null, null, null);


            ZipFolders(zipFileName, folderPath);

            return (await File.ReadAllBytesAsync(zipFileName), "application/zip", zipFileName);
        }






        /*
         * 
         * Download Multiple File with zip feature. Include all subfolders and files. 
         * Given parameters are List<MultipleFolderDownloadDto> and user uuid
         * Return the Triple structure and become the file on the controller.
         * 
         */
        public async Task<(byte[] bytes, string content, string fileName)> DownloadMultipleFolder(List<MultipleFolderDownloadDto> folderDownloadDto, Guid uid)
        {

            var folderIds = folderDownloadDto.Select(fid => fid.folderId).ToList();
            var folders = await m_folderRepositoryDal.FindByIdsAsync(folderIds);

            // Control the owner of each file is same
            if (!ControlFolderOwnersAsync(folders, uid))
                return (null, null, null);



            var folderPaths = folders.ToList().Select(f => Path.Combine(Util.DIRECTORY_BASE, f.FolderPath)).ToArray();

            if (!folderPaths.Any(fp => !Directory.Exists(Util.DIRECTORY_BASE + fp)))
                return (null, null, null);


            var zipFileName = DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss]") + "_downloaded_files.zip";


            ZipFolders(zipFileName, folderPaths);

            return (await File.ReadAllBytesAsync(zipFileName), "application/zip", zipFileName);
        }











        /*
         * 
         * If owner of selected files is valid, return true.
         * 
         */
        private async Task<bool> ControlFileOwnersAsync(IEnumerable<FileboxFile> files, Guid uid)
        {
            var usersFolders = await m_folderRepositoryDal.FindByFilterAsync(folder => folder.UserId == uid);
            var usersFolderIds = usersFolders.Select(folder => folder.FolderId).ToHashSet();

            return files.All(file => usersFolderIds.Contains(file.FolderId));
        }






        /*
         * 
         * Zip given folder (folder path) and given zip file a created by ZipFolders method.
         * 
         * Running the recursively and iteration of each subfolder, create new directory in zip then files of folders added to zip file.
         * 
         */
        public void AddFolderToZip(ZipArchive zipArchive, string sourceFolderPath, string parentFolderName)
        {
            string folderName = Path.GetFileName(sourceFolderPath);

            // Create an entry for the folder in the zip archive
            if (!string.IsNullOrEmpty(parentFolderName))
            {
                folderName = Path.Combine(parentFolderName, folderName);
            }
            zipArchive.CreateEntry(folderName + "/");

            // Add all files in the folder to the zip archive
            foreach (string filePath in Directory.GetFiles(sourceFolderPath))
            {
                string fileName = Path.GetFileName(filePath);
                zipArchive.CreateEntryFromFile(filePath, Path.Combine(folderName, fileName));
            }

            // Recursively add subfolders
            foreach (string subfolderPath in Directory.GetDirectories(sourceFolderPath))
            {
                AddFolderToZip(zipArchive, subfolderPath, folderName);
            }
        }






        /*
         * 
         * Zip folders with given name of zip file  and folderPaths parameters.
         * 
         * Trigger method for AddFolderToZip method.
         * 
         */
        private void ZipFolders(string zipFileName, params string[] folderPaths)
        {
            using (var zipArchive = ZipFile.Open(zipFileName, ZipArchiveMode.Create))
            {
                foreach (string folderPath in folderPaths)
                {
                    AddFolderToZip(zipArchive, folderPath, "");
                }
            }
        }






        /*
         * 
         * Check the owner of each folders are same. Compare it with given user uuid parameter
         * 
         * Return the boolean value if owner of each folders are same.
         * 
         */
        private bool ControlFolderOwnersAsync(IEnumerable<FileboxFolder> folders, Guid uid)
        {
            return folders.All(f => f.UserId == uid);
        }
    }
}
