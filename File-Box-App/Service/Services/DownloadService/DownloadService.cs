using AutoMapper;
using Azure;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using System.IO.Compression;
using System.Net.Mime;
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
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly IMapper m_mapper;


        public DownloadService(FileRepositoryDal fileRepositoryDal, FolderRepositoryDal folderRepositoryDal, UserRepositoryDal userRepositoryDal, IMapper mapper)
        {
            m_fileRepositoryDal = fileRepositoryDal;
            m_folderRepositoryDal = folderRepositoryDal;
            m_userRepositoryDal = userRepositoryDal;
            m_mapper = mapper;
        }
        /*
         * 
         * Download Single file.
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
         * Download Multiple files zipped.
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
         * If owner of selected files is valid, return true.
         * 
         */
        private async Task<bool> ControlFileOwnersAsync(IEnumerable<FileboxFile> files, Guid uid)
        {
            var usersFolders = await m_folderRepositoryDal.FindByFilterAsync(folder => folder.UserId == uid);
            var usersFolderIds = usersFolders.Select(folder => folder.FolderId).ToHashSet();

            return files.All(file => usersFolderIds.Contains(file.FolderId));
        }







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
         * Download Single folder zipped. Include subfolders
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
         * Download Multiple folder zipped. Include subfolders
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

            if (!folderPaths.Any(fp =>! Directory.Exists(Util.DIRECTORY_BASE + fp)))
                return (null, null, null);

            
            var zipFileName = DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss]") + "_downloaded_files.zip";


            ZipFolders(zipFileName, folderPaths);

            return (await File.ReadAllBytesAsync(zipFileName), "application/zip", zipFileName);
        }

        private bool ControlFolderOwnersAsync(IEnumerable<FileboxFolder> folders, Guid uid)
        {
            return folders.All(f => f.UserId == uid);
        }
    }
}
