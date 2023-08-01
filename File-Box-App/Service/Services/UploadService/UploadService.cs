using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace Service.Services.UploadService
{
    public class UploadService : IUploadService
    {
        private readonly IWebHostEnvironment m_webHostEnvironment;
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly FolderRepositoryDal m_folderRepositoryDal;
        private readonly FileRepositoryDal m_fileRepositoryDal;
        public UploadService(IWebHostEnvironment webHostEnvironment, UserRepositoryDal userRepositoryDal, FolderRepositoryDal folderRepositoryDal, FileRepositoryDal fileRepositoryDal)
        {
            m_webHostEnvironment = webHostEnvironment;
            m_userRepositoryDal = userRepositoryDal;
            m_folderRepositoryDal = folderRepositoryDal;
            m_fileRepositoryDal = fileRepositoryDal;
        }

        public async Task<bool> UploadMultipleFiles(List<IFormFile> formFiles, long folderId, Guid uid)
        {
            try
            {
                var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

                if (uid != folder.UserId) // If folder owner not the entered user
                    return false;

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
                }


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


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
                m_folderRepositoryDal.Save(folderBox);
            }


            var task =  UploadFilesAndSubFolders(sourcePath.sourceFilePath, expectedCreatingDirectory, folderId, folder.FolderPath + "\\" + targetFolderName, uid);

            await Task.WhenAll(task);
        }
        // destination path without copied folder name
        public async Task<bool> UploadSingleFolder(FolderUploadDto sourcePath, long folderId, Guid uid)
        {
            try
            {
                await UploadSingleFolderCallback(sourcePath, folderId, uid);  


                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

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
                await m_fileRepositoryDal.SaveAsync(new FileboxFile(fp.FolderId, Path.GetFileName(file),
                                                         Path.Combine(folderPath, Path.GetFileName(file)),
                                                         Path.GetExtension(file), new FileInfo(file).Length));
            }

            // Upload files in subdirectories
            foreach (var subdirectory in Directory.GetDirectories(sourcePath))
            {
                var subdirectoryName = Path.GetFileName(subdirectory);
                var subdirectoryTargetPath = Path.Combine(targetPath, subdirectoryName);
                var subdirectoryFolderPath = Path.Combine(folderPath, subdirectoryName);

                if (!Directory.Exists(subdirectoryTargetPath))
                {
                    Directory.CreateDirectory(subdirectoryTargetPath);
                    var folderBox = new FileboxFolder(folderId, uid, subdirectoryName, subdirectoryFolderPath);
                    FileboxFolder?  newFolder = m_folderRepositoryDal.Save(folderBox);
                    //subdirectoryTargetPath = Path.Combine(Util.DIRECTORY_BASE, newFolder.FolderPath, subdirectoryName);
                }

                

                await UploadFilesAndSubFolders(subdirectory, subdirectoryTargetPath, folderId, subdirectoryFolderPath, uid);
            }
        }

        public async Task<bool> UploadMultipleFolder(List<FolderUploadDto> sourcePaths, long folderId, Guid uid)
        {
            var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);
            
            foreach (var dir in sourcePaths)
            {
                await UploadSingleFolderCallback(dir, folderId, uid);
            }
            return true;
        }


        public async Task<bool> UploadSingleFile(IFormFile formFile, long folderId, Guid uid)
        {

            try
            {
                var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

                if (uid != folder.UserId) // If folder owner not the entered user
                    return false;

                var sourcePath = formFile.OpenReadStream();

                string targetPath = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath, formFile.FileName);

                using (var destinationStream = new FileStream(targetPath, FileMode.Create))
                {
                    await sourcePath.CopyToAsync(destinationStream);
                }

                var fileInfo = new FileInfo(targetPath);

                m_fileRepositoryDal.Save(new FileboxFile(folderId, formFile.FileName,
                                                         Path.Combine(folder.FolderPath, formFile.FileName),
                                                         fileInfo.Extension, fileInfo.Length));

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
