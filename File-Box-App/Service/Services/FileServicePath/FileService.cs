using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;

namespace Service.Services.FileServicePath
{
    public class FileService : IFileService
    {
        private readonly FolderRepositoryDal m_folderDal;
        private readonly FileRepositoryDal m_fileDal;
        private readonly IMapper m_mapper;
        private readonly UnitOfWork m_unitOfWork;
        public FileService(FileRepositoryDal fileDal,
                           FolderRepositoryDal folderDal,
                           IMapper mapper,
                           UnitOfWork unitOfWork)
        {
            m_fileDal = fileDal;
            m_folderDal = folderDal;
            m_mapper = mapper;
            m_unitOfWork = unitOfWork;
        }






        /*
         * 
         * 
         * Create Empty file with given FileSaveDto parameter.
         * 
         * 
         */
        public async Task<bool> CreateFile(FileSaveDto fileSaveDto)
        {
            var folder = await m_folderDal.FindByIdAsync(fileSaveDto.folderId);

            CheckFolderAndPermits(folder, Guid.Parse(fileSaveDto.userId));

            var fileFullPathDb = folder.FolderPath + "\\" + fileSaveDto.fileName;

            var realFilePath = Util.DIRECTORY_BASE + fileFullPathDb;

            if (File.Exists(realFilePath))
                throw new ServiceException($"File already exists on {fileFullPathDb}");

            var fs = File.Create(realFilePath);

            fs.Close();

            await m_fileDal.Save(new FileboxFile(fileSaveDto.folderId, fileSaveDto.fileName, fileFullPathDb, fileSaveDto.fileType, 0));
            await m_fileDal.SaveChangesAsync();
            return true;
        }






        /*
         * 
         * 
         * Remove file with given file Id
         * 
         * 
         */
        public async Task<string> DeleteFile(long fileId, Guid guid)
        {
            try
            {
                var file = await m_fileDal.FindByIdAsync(fileId);
                var folder = await m_folderDal.FindByIdAsync(file.FolderId);
                CheckFolderAndPermits(folder, guid);
                File.Delete(Util.DIRECTORY_BASE + file.FilePath);
                m_fileDal.DeleteById(fileId);

                return file.FileName;
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceException("Arguments are null!");
            }
            catch (ArgumentException ex)
            {
                throw new ServiceException("Arguments are wrong!");
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ServiceException("Directory Not Found!");
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ServiceException("You do not have permits for deleting the file!");
            }
            catch (PathTooLongException ex)
            {
                throw new ServiceException("Path too long! Please attention to path length!");
            }
            catch (IOException ex)
            {
                throw new ServiceException("Files does not deleted!");
            }
        }





        /*
         * 
         * 
         * Remove files with given file Ids
         * 
         * 
         */
        public async Task<List<FileViewDto>> DeleteMultipleFile(List<long> fileIds, Guid guid)
        {


            try
            {
                var files = (await m_unitOfWork.FileRepository.FindByFilterAsync(f => fileIds.Contains(f.FileId))).ToList();

                var folder = (await m_unitOfWork.FolderRepository.FindByFilterAsync(f => f.FolderId == files[0].FolderId)).Single();

                CheckFolderAndPermits(folder, guid);

                foreach (var file in files)
                    File.Delete(Util.DIRECTORY_BASE + file.FilePath);

                await m_unitOfWork.FileRepository.RemoveAll(files);

                var dtoList = new List<FileViewDto>();

                foreach (var file in files)
                    dtoList.Add(m_mapper.Map<FileViewDto>(file));


                m_unitOfWork.Save();

                return dtoList;
            }
            catch (ArgumentNullException ex)
            {
                throw new ServiceException("Arguments are null!");
            }
            catch (ArgumentException ex)
            {
                throw new ServiceException("Arguments are wrong!");
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new ServiceException("Directory Not Found!");
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ServiceException("You do not have permits for deleting the file!");
            }
            catch (PathTooLongException ex)
            {
                throw new ServiceException("Path too long! Please attention to path length!");
            }
            catch (IOException ex)
            {
                throw new ServiceException("Files does not deleted!");
            }

        }







        /*
         * 
         * 
         * Rename file and update its filepath and updated date informations with given fileId and new File Name.
         * 
         * 
         */
        public async Task<FileViewDto> RenameFile(long fileId, string newFileName, Guid userId)
        {
            try
            {
                newFileName = Util.ConvertToEnglishCharacters(newFileName);

                var fileObj = await m_unitOfWork.FileRepository.FindByIdAsync(fileId);

                if (fileObj == null)
                    throw new ServiceException("File not found!");

                var folder = await m_unitOfWork.FolderRepository.FindByIdAsync(fileObj.FolderId);

                CheckFolderAndPermits(folder, userId);

                var oldFileName = fileObj.FileName;
                var pathWithoutFolderName = Path.GetDirectoryName(fileObj.FilePath);
                var newFilePath = Path.Combine(pathWithoutFolderName, newFileName);

                File.Move(Path.Combine(Util.DIRECTORY_BASE, fileObj.FilePath), Path.Combine(Util.DIRECTORY_BASE, newFilePath));

                fileObj.FileName = newFileName;
                fileObj.FilePath = newFilePath;
                fileObj.UpdatedDate = DateTime.Now;

                m_unitOfWork.FileRepository.Update(fileObj);
                m_unitOfWork.Save();

                return m_mapper.Map<FileViewDto>(fileObj);
            }
            catch (FileNotFoundException ex)
            {
                throw new ServiceException("File not found on directory!");
            }
        }









        /*
         * 
         * Get all files from db with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        public async Task<IEnumerable<FileViewDto>> GetFilesByFolderIdAsync(long folderId, Guid userId, string currentToken)
        {
            var folder = await m_folderDal.FindByIdAsync(folderId);

            CheckFolderAndPermits(folder, userId);

            var filteredFiles = await m_fileDal.FindByFilterAsync(file => file.FolderId == folderId);

            if (filteredFiles is null)
                throw new ServiceException("Folder does not have any file!");

            return filteredFiles.Select(file => m_mapper.Map<FileViewDto>(file));
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
         * Get all files from db with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        public async Task<IEnumerable<FileViewDto>> GetFilesByFileExtensionAndFolderIdAsync(long folderId, string? fileExtension, Guid userId)
        {
            var folder = await m_folderDal.FindByIdAsync(folderId); // error on folder id

            CheckFolderAndPermits(folder, userId);

            var filteredFiles = await m_fileDal.FindByFilterAsync(file => file.FolderId == folderId && file.FileType.ToLower() == fileExtension.ToLower());

            return filteredFiles.Select(file => m_mapper.Map<FileViewDto>(file));
        }





        /*
         * 
         * Sort files with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        public async Task<IEnumerable<FileViewDto>> SortFilesByFileBytesAsync(long folderId, Guid userId)
        {
            var folder = await m_folderDal.FindByIdAsync(folderId);

            CheckFolderAndPermits(folder, userId);

            var files = await m_fileDal.FindByFilterAsync(file => file.FolderId == folderId);
            var sortedFiles = await Task.Run(() => files.OrderByDescending(file => file.FileSize));

            return sortedFiles.Select(file => m_mapper.Map<FileViewDto>(file));
        }





        /*
         * 
         * Sort files with given folder ıd and user about creation date
         * 
         * 
         */
        public async Task<IEnumerable<FileViewDto>> SortFilesByCreationDateAsync(long folderId, Guid userId)
        {
            var folder = await m_folderDal.FindByIdAsync(folderId);

            CheckFolderAndPermits(folder, userId);

            var files = await m_fileDal.FindByFilterAsync(file => file.FolderId == folderId);
            var sortedFiles = await Task.Run(() => files.OrderBy(file => file.CreatedDate));

            return sortedFiles.Select(file => m_mapper.Map<FileViewDto>(file));
        }




        /*
         * 
         * Find File with given parameter is file id and user id 
         * 
         */
        public async Task<FileViewDto> FindFileByFileId(long fileId, Guid userId)
        {
            var file = await m_fileDal.FindById(fileId);
            var folder = await m_folderDal.FindByIdAsync(file.FolderId);

            CheckFolderAndPermits(folder, userId);

            return m_mapper.Map<FileViewDto>(file);
        }


        private async Task<string> GetNewFileName(UnitOfWork unitOfWork, FileboxFolder targetFolder, FileboxFile file)
        {
            var currentFilesOnFolder = await unitOfWork.FileRepository.FindByFilterAsync(f => f.FolderId == targetFolder.FolderId);

            var SameFileListByName = currentFilesOnFolder.Where(f => f.FileName.Contains(Path.GetFileNameWithoutExtension(file.FileName))).ToList();

            string newFileName = Path.GetFileNameWithoutExtension(file.FileName);

            if (SameFileListByName.Count != 0) // If exists same file
            {
                var count = SameFileListByName.Count;

                newFileName += $"({count})";
            }
            newFileName += Path.GetExtension(file.FileName);

            return Util.ConvertToEnglishCharacters(newFileName);
        }


        public async Task<FileViewDto> CopyFileToAnotherFolder(long fileId, long targetFolderId, Guid userId)
        {
            try
            {
                var file = await m_unitOfWork.FileRepository.FindByIdAsync(fileId);
                var currentFolder = await m_unitOfWork.FolderRepository.FindByIdAsync(file.FolderId);
                var targetFolder = await m_unitOfWork.FolderRepository.FindByIdAsync(targetFolderId);


                var newFileName = await GetNewFileName(m_unitOfWork, targetFolder, file);


                CheckFolderAndPermits(currentFolder, userId);
                CheckFolderAndPermits(targetFolder, userId);

                var copiedFile = new FileboxFile(targetFolderId, newFileName, Path.Combine(targetFolder.FolderPath, newFileName), file.FileType, file.FileSize);

                // Add to database
                await m_unitOfWork.FileRepository.SaveAsync(copiedFile);

                var sourcePath = new FileInfo(Path.Combine(Util.DIRECTORY_BASE, file.FilePath));

                using (var sourceStream = sourcePath.OpenRead())
                using (var destinationStream = new FileStream(Path.Combine(Util.DIRECTORY_BASE, targetFolder.FolderPath, copiedFile.FileName), FileMode.Create))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                m_unitOfWork.Save();

                return m_mapper.Map<FileViewDto>(copiedFile);
            }
            catch
            {
                throw;
            }
        }





        public async Task<FileViewDto> MoveFileToAnotherFolder(long fileId, long targetFolderId, Guid userId)
        {
            try
            {
                var file = await m_unitOfWork.FileRepository.FindByIdAsync(fileId);
                var currentFolder = await m_unitOfWork.FolderRepository.FindByIdAsync(file.FolderId);
                var targetFolder = await m_unitOfWork.FolderRepository.FindByIdAsync(targetFolderId);

                CheckFolderAndPermits(currentFolder, userId);
                CheckFolderAndPermits(targetFolder, userId);

                var newFileName = await GetNewFileName(m_unitOfWork, targetFolder, file);

                var copiedFile = new FileboxFile(targetFolderId, newFileName, Path.Combine(targetFolder.FolderPath, newFileName), file.FileType, file.FileSize);

                await m_unitOfWork.FileRepository.SaveAsync(copiedFile);

                var sourcePath = Path.Combine(Util.DIRECTORY_BASE, file.FilePath);
                var targetPath = Path.Combine(Util.DIRECTORY_BASE, targetFolder.FolderPath, copiedFile.FileName);

                await MoveFileAsync(sourcePath, targetPath);

                await m_unitOfWork.FileRepository.Delete(file);
                m_unitOfWork.Save();

                return m_mapper.Map<FileViewDto>(copiedFile);
            }
            catch
            {
                throw;
            }
        }

        private async Task MoveFileAsync(string sourcePath, string targetPath)
        {
            var task = new Task(() => File.Move(sourcePath, targetPath));
            task.Start();
            await Task.WhenAll(task);
        }

    }
}
