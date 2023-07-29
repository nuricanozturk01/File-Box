using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace Service.Services.FileServicePath
{
    public class FileService : IFileService
    {
        private readonly FolderRepositoryDal m_folderDal;
        private readonly FileRepositoryDal m_fileDal;

        public FileService(FileRepositoryDal fileDal, FolderRepositoryDal folderDal)
        {
            m_fileDal = fileDal;
            m_folderDal = folderDal;
        }

        public async Task<bool> CreateFile(FileSaveDto fileSaveDto)
        {
            var folder = await m_folderDal.FindByIdAsync(fileSaveDto.folderId);
            
            var fileFullPathDb = folder.FolderPath + "\\" + fileSaveDto.fileName;

            var realFilePath = Util.DIRECTORY_BASE + fileFullPathDb;
            
            if (File.Exists(realFilePath))
                return false;

            var fs = File.Create(realFilePath);

            fs.Close();
            
            m_fileDal.Save(new FileboxFile(fileSaveDto.folderId, fileSaveDto.fileName, fileFullPathDb, fileSaveDto.fileType, 0));
            
            return true;
        }

        public async Task<bool> DeleteFile(long folderId)
        {
            var folder = await m_fileDal.FindByIdAsync(folderId);

            File.Delete(Util.DIRECTORY_BASE + folder.FilePath);
            m_fileDal.DeleteById(folderId);

            return true;
        }

        public async Task<IEnumerable<FileboxFile>?> GetFilesByUserIdAsync(Guid userId, long folderId)
        {
            return await m_fileDal.FindByFilterAsync(f => f.FolderId == folderId);
        }

        public void RenameFile(long fileId, string newFileName)
        {
            var fileObj = m_fileDal.FindById(fileId);
            var pathWithoutFolderName = Path.GetDirectoryName(fileObj.FilePath);
            var newFilePath = Path.Combine(pathWithoutFolderName, newFileName);

            File.Move(Util.DIRECTORY_BASE + fileObj.FilePath, Util.DIRECTORY_BASE + newFilePath);
            fileObj.FileName = newFileName;
            fileObj.FilePath = newFilePath;

            m_fileDal.Update(fileObj);
        }
    }
}
