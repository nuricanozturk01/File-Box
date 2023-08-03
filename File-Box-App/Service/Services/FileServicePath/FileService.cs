using AutoMapper;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using Service.Exceptions;

namespace Service.Services.FileServicePath
{
    public class FileService : IFileService
    {
        private readonly FolderRepositoryDal m_folderDal;
        private readonly FileRepositoryDal m_fileDal;
        private readonly IMapper m_mapper;

        public FileService(FileRepositoryDal fileDal, FolderRepositoryDal folderDal, IMapper mapper)
        {
            m_fileDal = fileDal;
            m_folderDal = folderDal;
            m_mapper = mapper;
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

            m_fileDal.Save(new FileboxFile(fileSaveDto.folderId, fileSaveDto.fileName, fileFullPathDb, fileSaveDto.fileType, 0));

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
         * Rename file and update its filepath and updated date informations with given fileId and new File Name.
         * 
         * 
         */
        public async Task<string> RenameFile(long fileId, string newFileName, Guid userId)
        {

            try
            {
                var fileObj = m_fileDal.FindById(fileId);

                if (fileObj is null)
                    throw new ServiceException("File not found!");

                var folder = await m_folderDal.FindByIdAsync(fileObj.FolderId);

                CheckFolderAndPermits(folder, userId);

                var oldFileName = fileObj.FileName;

                var pathWithoutFolderName = Path.GetDirectoryName(fileObj.FilePath);
                var newFilePath = Path.Combine(pathWithoutFolderName, newFileName);

                File.Move(Util.DIRECTORY_BASE + fileObj.FilePath, Util.DIRECTORY_BASE + newFilePath);

                fileObj.FileName = newFileName;
                fileObj.FilePath = newFilePath;
                fileObj.UpdatedDate = DateTime.Now;

                m_fileDal.Update(fileObj);

                return oldFileName;
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
        public async Task<IEnumerable<FileViewDto>> GetFilesByFolderIdAsync(long folderId, Guid userId)
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
            var folder = await m_folderDal.FindByIdAsync(folderId);

            CheckFolderAndPermits(folder, userId);

            var filteredFiles = await m_fileDal.
                FindByFilterAsync(file => file.FolderId == folderId && file.FileType.ToLower() == fileExtension.ToLower());

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
    }
}
