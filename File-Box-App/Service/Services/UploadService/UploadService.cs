using Microsoft.AspNetCore.Http;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using Service.Exceptions;

namespace Service.Services.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly FolderRepositoryDal m_folderRepositoryDal;
        private readonly FileRepositoryDal m_fileRepositoryDal;
        public UploadService(FolderRepositoryDal folderRepositoryDal, FileRepositoryDal fileRepositoryDal)
        {
            m_folderRepositoryDal = folderRepositoryDal;
            m_fileRepositoryDal = fileRepositoryDal;
        }






        /*
         * 
         * Upload multiple files with using compressing
         * returns the boolean value that if success return true else return false
         * 
         */
        public async Task<(string path, long totalLength)> UploadMultipleFiles(List<IFormFile> formFiles, long folderId, Guid uid)
        {
            try
            {
                var totalBytes = formFiles.Select(ff => ff.Length).Sum();

                if (totalBytes > Util.MAX_BYTE_UPLOAD_MULTIPLE_FILE)
                    throw new ServiceException("Maximum Uplodaed single file limit is " + Util.ByteToMB(Util.MAX_BYTE_UPLOAD_MULTIPLE_FILE) + " MB");
                
                var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

                CheckFolderAndPermits(folder, uid);

                foreach (var ff in formFiles)
                {
                    var sourcePath = ff.OpenReadStream();

                    string targetPath = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath, ff.FileName);

                    using (var destinationStream = new FileStream(targetPath, FileMode.Create))
                    {
                        await sourcePath.CopyToAsync(destinationStream);
                    }

                    var fileInfo = new FileInfo(targetPath);


                    await m_fileRepositoryDal.SaveAsync(new FileboxFile(folderId, ff.FileName,
                                                             Path.Combine(folder.FolderPath, ff.FileName),
                                                             fileInfo.Extension, fileInfo.Length));
                    await m_fileRepositoryDal.SaveChangesAsync();
                }

                return (Path.Combine(folder.FolderPath), Util.ByteToMB(totalBytes));
            }
            catch (ServiceException ex)
            {
                throw new ServiceException(ex.GetMessage);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Something wrong on upload files!");
            }
        }











        /*
         * 
         * Upload single folder callback with using compressing. Using for multiple and single uploads.
         * 
         * 
         */
        private async Task UploadSingleFolderCallback(FolderUploadDto sourcePath, long folderId, Guid uid)
        {

            var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

            var targetFolderName = Path.GetFileName(sourcePath.sourceFilePath);

            var expectedCreatingDirectory = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath, targetFolderName);

            if (uid != folder.UserId) // If folder owner not the entered user
                return;

            if (!Directory.Exists(expectedCreatingDirectory))
            {
                Directory.CreateDirectory(expectedCreatingDirectory); // directory created.
                var folderBox = new FileboxFolder(folderId, uid, targetFolderName, folder.FolderPath + "\\" + targetFolderName);
                await m_folderRepositoryDal.Save(folderBox);
                await m_folderRepositoryDal.SaveChangesAsync();
            }


            var task = UploadFilesAndSubFolders(sourcePath.sourceFilePath, expectedCreatingDirectory, folderId, folder.FolderPath + "\\" + targetFolderName, uid);

            await Task.WhenAll(task);
        }











        /*
         * 
         * Zip uploaded files and folders include subfolders and their files. (recursively)
         * 
         * 
         */
        private async Task UploadFilesAndSubFolders(string sourcePath, string targetPath, long folderId, string folderPath, Guid uid)
        {
            foreach (var file in Directory.GetFiles(sourcePath))
            {
                using (FileStream sourceFileStream = File.OpenRead(file))
                {
                    var targetFilePath = Path.Combine(targetPath, Path.GetFileName(file));

                    if (File.Exists(targetFilePath))
                        File.Delete(targetFilePath);

                    using (FileStream outputFileStream = File.Create(targetFilePath))
                    {
                        await sourceFileStream.CopyToAsync(outputFileStream);
                    }
                }

                var fp = m_folderRepositoryDal.FindByFilterAsync(f => f.FolderPath == folderPath).Result.FirstOrDefault();

                await m_fileRepositoryDal.Save(new FileboxFile(fp.FolderId, Path.GetFileName(file),
                                                         Path.Combine(folderPath, Path.GetFileName(file)),
                                                         Path.GetExtension(file), new FileInfo(file).Length));
                await m_fileRepositoryDal.SaveChangesAsync();
            }

            foreach (var subdirectory in Directory.GetDirectories(sourcePath))
            {
                var subdirectoryName = Path.GetFileName(subdirectory);
                var subdirectoryTargetPath = Path.Combine(targetPath, subdirectoryName);
                var subdirectoryFolderPath = Path.Combine(folderPath, subdirectoryName);

                if (!Directory.Exists(subdirectoryTargetPath))
                {
                    Directory.CreateDirectory(subdirectoryTargetPath);
                    var folderBox = new FileboxFolder(folderId, uid, subdirectoryName, subdirectoryFolderPath);
                    await m_folderRepositoryDal.Save(folderBox);
                    await m_folderRepositoryDal.SaveChangesAsync();
                }



                await UploadFilesAndSubFolders(subdirectory, subdirectoryTargetPath, folderId, subdirectoryFolderPath, uid);
            }
        }











        /*
         * 
         * Upload multiple folders with using compressing. But using on single folder. list length is one
         * returns the boolean value that if success return true else return false
         * 
         */
        public async Task<List<FolderUploadDto>> UploadMultipleFolder(List<FolderUploadDto> sourcePaths, long folderId, Guid uid)
        {
            var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

            CheckFolderAndPermits(folder, uid);
            CheckDirectoriesAreExists(sourcePaths);

            foreach (var dir in sourcePaths)
                await UploadSingleFolderCallback(dir, folderId, uid);

            return sourcePaths;
        }

        private void CheckDirectoriesAreExists(List<FolderUploadDto> sourcePaths)
        {
            if (!sourcePaths.All(dir => Directory.Exists(dir.sourceFilePath)))
                throw new ServiceException("Please control the paths...");
        }








        /*
         * 
         * Check the folder and folder owner is same with user Id
         * 
         * 
         */
        private void CheckFolderAndPermits(FileboxFolder folder, Guid userId)
        {
            if (folder is null)
                throw new ServiceException("Folder is not found!");

            if (folder.UserId != userId)
                throw new ServiceException("You cannot access this folder!");
        }






        /*
         * 
         * Upload single file
         * returns the boolean value that if success return true else return false
         * 
         */
        public async Task<(string path, long totalLength)> UploadSingleFile(IFormFile formFile, long folderId, Guid uid)
        {
            try
            {
                if (formFile.Length > Util.MAX_BYTE_UPLOAD_SINGLE_FILE)
                    throw new ServiceException("Maximum Uplodaed single file limit is " + Util.ByteToMB(Util.MAX_BYTE_UPLOAD_SINGLE_FILE));

                var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

                CheckFolderAndPermits(folder, uid);

                var sourcePath = formFile.OpenReadStream();
                
                string targetPath = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath, formFile.FileName);

                using (var destinationStream = new FileStream(targetPath, FileMode.Create))
                {
                    await sourcePath.CopyToAsync(destinationStream);
                }

                var fileInfo = new FileInfo(targetPath);

               await m_fileRepositoryDal.Save(new FileboxFile(folderId, formFile.FileName,
                                                         Path.Combine(folder.FolderPath, formFile.FileName),
                                                         fileInfo.Extension, fileInfo.Length));
                await m_fileRepositoryDal.SaveChangesAsync();
                return (Path.Combine(folder.FolderPath, formFile.FileName), Util.ByteToMB(formFile.Length));
            }
            catch (Exception ex)
            {
                throw new ServiceException("Something wrong on uploading file!");
            }
        }
    }
}
