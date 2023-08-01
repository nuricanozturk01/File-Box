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



        // destination path without copied folder name
        public async Task<bool> UploadSingleFolder(FolderUploadDto sourcePath, long folderId, Guid uid)
        {
            try
            {
                var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);

                var targetFolderName = Path.GetFileName(sourcePath.sourceFilePath);

                var expectedCreatingDirectory = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath, targetFolderName);

                if (uid != folder.UserId) // If folder owner not the entered user
                    return false;

                if (!Directory.Exists(expectedCreatingDirectory))
                    Directory.CreateDirectory(expectedCreatingDirectory); // directory created.

                var sourceDirectory = new DirectoryInfo(sourcePath.sourceFilePath);

                // All files on specific directory
                foreach (var file in sourceDirectory.GetFiles())
                {
                    using (FileStream sourceFileStream = file.OpenRead())
                    {
                        var targetFilePath = Path.Combine(expectedCreatingDirectory, file.Name);
                        if (File.Exists(targetFilePath))
                            File.Delete(targetFilePath);

                        using (FileStream outputFileStream = File.Create(targetFilePath))
                        {
                            await sourceFileStream.CopyToAsync(outputFileStream);
                        }
                    }

                    await m_fileRepositoryDal.SaveAsync(new FileboxFile(folderId, file.Name,
                                                             Path.Combine(folder.FolderPath, file.Name),
                                                             file.Extension, file.Length));
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


        public async Task<bool> UploadMultipleFolder(List<FolderUploadDto> sourcePaths, long folderId, Guid uid)
        {
            var folder = await m_folderRepositoryDal.FindByIdAsync(folderId);
            sourcePaths.ForEach(sp => UploadSingleFolder(sp, folderId, uid));
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
