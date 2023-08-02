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

            m_folderDal.Save(fileBoxFolder);

            if (!Directory.Exists(fullName))
                Directory.CreateDirectory(fullName);

            return true;
        }
        





    
        /*
         * 
         * 
         * Find all subfolders given folder and sort it by folderPath length so,
         * deleted folder starting with leaf node.
         * I am think thinking using the dfs algorithm.
         * 
         * 
         */
        private IEnumerable<FileboxFolder> GetAllSubFolders(FileboxFolder folder)
        {
            return m_folderDal
                .FindByFilterAsync(f => f.FolderId >= folder.FolderId && f.FolderPath.Contains(folder.FolderName))
                .Result
                .OrderByDescending(f => f.FolderPath.Length);
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
            
            var deletedFolderWithSubFolders = GetAllSubFolders(dir);

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
                    m_folderDal.Delete(folder);
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
            var folders = m_folderDal.AllFolders.Where(x => x.UserId == userId).ToList();

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

                var oldPath = folder.FolderPath; //nuricanozturk/dev/nuri
                var oldName = folder.FolderName; //nuri

                // nuricanozturk/dev/nuri     new name: can
                var oldPathParent = Path.GetDirectoryName(folder.FolderPath); // without last folder (nuricanozturk/dev)
                var newFullPath = Path.Combine(oldPathParent, newFolderName); // nuricanozturk/dev/can

                // Rename folder
                Directory.Move(Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath), Path.Combine(Util.DIRECTORY_BASE, newFullPath));

                // Update parent folder
                folder.FolderName = newFolderName;
                folder.FolderPath = newFullPath;
                folder.UpdatedDate = DateTime.Now;


                // Update subfolders
                await UpdateSubFolders(oldPath, oldName, newFolderName);

                // Update files on parent folder
                await UpdateFiles(folderId, oldName, newFolderName);

                return (oldPath, newFullPath);
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
            catch (ServiceException ex)
            {
                throw new ServiceException(ex.GetMessage);
            }
        }






        /*
         * 
         * 
         * Update subfolders when rename parent folder
         * 
         * 
         */
        private async Task UpdateSubFolders(string oldPath, string oldName, string newFolderName)
        {
            var subfolders = await m_folderDal.FindByFilterAsync(f => f.FolderPath.Contains(oldPath));
            
            if (subfolders is null)
                throw new ServiceException("Subfolders are null!");

            foreach (var subfolder in subfolders)
            {
                subfolder.UpdatedDate = DateTime.Now;
                subfolder.FolderPath = subfolder.FolderPath.Replace(oldName, newFolderName);
            }

            await m_folderDal.UpdateAll(subfolders);
        }







        /*
         * 
         * 
         * update files when rename parent folder
         * 
         * 
         */
        private async Task UpdateFiles(long folderId, string oldName, string newFolderName)
        {
            var files = await m_fileRepositoryDal.FindByFilterAsync(file => file.FolderId == folderId);
            
            if (files is null)
                throw new ServiceException("Files are null!");
            
            foreach (var file in files)
            {
                file.FilePath = file.FilePath.Replace(oldName, newFolderName);
                file.UpdatedDate = DateTime.Now;
            }
            await m_fileRepositoryDal.UpdateAll(files);
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
    }
}