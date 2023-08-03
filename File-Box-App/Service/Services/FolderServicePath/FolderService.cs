using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using Service.Exceptions;

namespace Service.Services.FolderService
{
    public class FolderService : IFolderService
    {
        private readonly FolderRepositoryDal m_folderDal;
        private readonly FileRepositoryDal m_fileRepositoryDal;
        private readonly IMapper m_mapper;

        public FolderService(FolderRepositoryDal folderDal, IMapper mapper, FileRepositoryDal fileRepositoryDal)
        {
            m_folderDal = folderDal;
            m_mapper = mapper;
            m_fileRepositoryDal = fileRepositoryDal;
        }






        /*
         * 
         * 
         * Find Parent Folder Id given current folder path.
         * 
         * 
         */
        public long? GetParentFolderId(string currentfolder)
        {
            var folder = m_folderDal.FindByFilterAsync(f => f.FolderPath == currentfolder).Result.FirstOrDefault();

            return folder == null ? null : folder.FolderId;
        }






        /*
         * 
         * 
         * Create Empty folder with given FolderSaveDTO parameter.
         * 
         * 
         */
        public async Task<bool> CreateFolder(FolderSaveDTO folderSaveDto)
        {
            var userUID = Guid.Parse(folderSaveDto.userId); // user uuid

            var folderNameWithoutPath = folderSaveDto.newFolderName; // just folder name


            var parentFolderPath = Util.DIRECTORY_BASE + folderSaveDto.currentFolderPath;

            var userFolderPath = folderSaveDto.currentFolderPath + "\\" + folderSaveDto.newFolderName;

            var fullName = parentFolderPath + "\\" + folderSaveDto.newFolderName;

            var fileBoxFolder = new FileboxFolder(GetParentFolderId(folderSaveDto.currentFolderPath), userUID, folderNameWithoutPath, userFolderPath);

            await m_folderDal.Save(fileBoxFolder);
            await m_folderDal.SaveChangesAsync();
            if (!Directory.Exists(fullName))
                Directory.CreateDirectory(fullName);

            return true;
        }







        /*
         * 
         * 
         * Find all subfolders given folder and sort it by folderPath length so,
         * 
         * 
         */
        private async Task<IEnumerable<FileboxFolder>> GetAllSubFolders(FileboxFolder folder)
        {
            var folders = await m_folderDal.FindByFilterAsync(f => f.FolderId >= folder.FolderId && f.FolderPath.Contains(folder.FolderName));

            return folders.OrderByDescending(f => f.FolderPath.Length);
        }






        /*
         * 
         * 
         * Remove folder with given folderId parameter
         * 
         * 
         */
        public async Task<string> DeleteFolder(long folderId, Guid userID)
        {
            var dir = await m_folderDal.FindByIdAsync(folderId);

            CheckFolderAndPermits(dir, userID);

            var deletedFolderWithSubFolders = await GetAllSubFolders(dir);

            CheckFolderExistsIfNotRemoveIt(deletedFolderWithSubFolders);

            if (!deletedFolderWithSubFolders.IsNullOrEmpty())
            {
                m_folderDal.RemoveAll(deletedFolderWithSubFolders);

                deletedFolderWithSubFolders.ToList().ForEach(f => Directory.Delete(Util.DIRECTORY_BASE + f.FolderPath, true));
            }

            return dir.FolderPath;
        }

        private void CheckFolderExistsIfNotRemoveIt(IEnumerable<FileboxFolder> deletedFolderWithSubFolders)
        {
            foreach (var folder in deletedFolderWithSubFolders)
            {
                var fullPathOnSystem = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath);

                if (!Directory.Exists(fullPathOnSystem))
                {
                    m_folderDal.Delete(folder);
                    m_folderDal.SaveChanges();
                }
            }
        }






        /*
         * 
         * 
         * Get Folders for specific user
         * 
         * 
         */
        public async Task<IEnumerable<FolderViewDto>> GetFoldersByUserIdAsync(Guid userId)
        {
            var folders = (await m_folderDal.FindAllAsync()).Where(x => x.UserId == userId).ToList();

            if (folders is null)
                throw new ServiceException("Folders are null!");

            var folderViewDtos = folders.Select(folder => m_mapper.Map<FolderViewDto>(folder)).ToList();

            return await Task.FromResult(folderViewDtos);
        }






        /*
         * 
         * 
         * Rename Folder and update file and folder paths its files, subfolders and their files.
         * 
         * 
         */
        public async Task<(string oldPath, string newPath)> RenameFolder(long folderId, string newFolderName, Guid userId)
        {
            try
            {


                var folder = await m_folderDal.FindByIdAsync(folderId); // find folder by id

                CheckFolderAndPermits(folder, userId);

                if (folder.ParentFolderId is null)
                    throw new ServiceException("Root folder name cannot be changed!");

                var oldFolderPathStartsWithRoot = folder.FolderPath; //"nuricanozturk\\Dotnet\\Dotnet"
                var oldFolderName = folder.FolderName; //"Dotnet"

                // nuricanozturk/dev/nuri     new name: can
                var oldFolderPathWithoutOldName = Path.GetDirectoryName(folder.FolderPath); // "nuricanozturk\\Dotnet"
                var newFolderPathStartsWithRoot = Path.Combine(oldFolderPathWithoutOldName, newFolderName); // "nuricanozturk\\Dotnet\\ASD"

                // Rename folder
                Directory.Move(Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath), Path.Combine(Util.DIRECTORY_BASE, newFolderPathStartsWithRoot));

                // Update parent folder
                folder.FolderName = newFolderName;
                folder.FolderPath = newFolderPathStartsWithRoot;
                folder.UpdatedDate = DateTime.Now;

                m_folderDal.Update(folder);
                m_folderDal.SaveChanges();
                var newFullPathOnSystem = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath);

                // Update files of parent folder
                var files = (await m_fileRepositoryDal.FindAllAsync()).Where(file => file.FilePath.Contains(oldFolderPathStartsWithRoot)).ToList();

                files.ForEach(file => file.FilePath = file.FilePath.Replace(oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot));

                await m_fileRepositoryDal.UpdateAll(files);

                var folders = (await m_folderDal.FindAllAsync()).Where(f => f.FolderPath.Contains(oldFolderPathStartsWithRoot)).ToList();

                folders.ForEach(f => f.FolderPath = f.FolderPath.Replace(oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot));

                await m_folderDal.UpdateAll(folders);


                return (oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot);
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
                throw new ServiceException(ex.Message);
            }
            catch (ServiceException ex)
            {
                throw new ServiceException(ex.GetMessage);
            }
        }






        /*
         * 
         * 
         * Find Root path with given userid parameter.
         * 
         * 
         */
        public async Task<FolderViewDto> FindRootFolder(Guid guid)
        {
            var folder = (await GetFoldersByUserIdAsync(guid)).FirstOrDefault();

            CheckFolderAndPermits(folder, guid);

            return folder;
        }








        /*
         * 
         * Check the folder and folder owner is same with user Id
         * 
         * 
         */
        private void CheckFolderAndPermits(FolderViewDto folder, Guid userId)
        {
            if (folder is null)
                throw new ServiceException("Folder is not found!");

            if (Guid.Parse(folder.userId) != userId)
                throw new ServiceException("You cannot access this folder!");
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

        public async Task<IEnumerable<FoldersWithFilesDto>> FindFolderWithFiles(Guid guid)
        {
            var folders = await m_folderDal.FindFoldersByUserId(guid); // IEnumerableFileboxFolder
            
            var folderWithFiles = new List<FoldersWithFilesDto>();

            foreach (var folder in folders)
            {
               
                var files = await m_fileRepositoryDal.FindFilesByFolderId(folder.FolderId); // files on folder

                var dto = new FoldersWithFilesDto(folder.FolderName, folder.FolderPath, folder.CreationDate, folder.FolderId, folder.UserId.ToString(), folder.ParentFolderId, files.Select(f => new FileViewDto(f.FileName, f.FileType, f.FileSize, f.FilePath, f.CreatedDate, f.UpdatedDate)).ToList());

                folderWithFiles.Add(dto);
            }

            return folderWithFiles;
            
        }
    }
}