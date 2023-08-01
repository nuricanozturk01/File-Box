using AutoMapper;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;

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
            
            var fileFullPathDb = folder.FolderPath + "\\" + fileSaveDto.fileName;

            var realFilePath = Util.DIRECTORY_BASE + fileFullPathDb;
            
            if (File.Exists(realFilePath))
                return false;

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
        public async Task<bool> DeleteFile(long fileId)
        {
            var folder = await m_fileDal.FindByIdAsync(fileId);

            File.Delete(Util.DIRECTORY_BASE + folder.FilePath);
            m_fileDal.DeleteById(fileId);

            return true;
        }






        /*
         * 
         * 
         *  Return the all files on specific folder with given parameters userId and folderId
         * 
         * 
         */
        public async Task<IEnumerable<FileViewDto>?> GetFilesByUserIdAndFolderIdAsync(Guid userId, long folderId)
        {
            var files =  await m_fileDal.FindByFilterAsync(f => f.FolderId == folderId);

            return files.AsParallel().Select(file => m_mapper.Map<FileViewDto>(file));
        }






        /*
         * 
         * 
         * Rename file and update its filepath and updated date informations with given fileId and new File Name.
         * 
         * 
         */
        public void RenameFile(long fileId, string newFileName)
        {
            var fileObj = m_fileDal.FindById(fileId);
            
            var pathWithoutFolderName = Path.GetDirectoryName(fileObj.FilePath);
            var newFilePath = Path.Combine(pathWithoutFolderName, newFileName);

            File.Move(Util.DIRECTORY_BASE + fileObj.FilePath, Util.DIRECTORY_BASE + newFilePath);
            
            fileObj.FileName = newFileName;
            fileObj.FilePath = newFilePath;
            fileObj.UpdatedDate = DateTime.Now;

            m_fileDal.Update(fileObj);
        }
    }
}
