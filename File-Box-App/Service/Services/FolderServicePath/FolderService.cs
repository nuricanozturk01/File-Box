using AutoMapper;
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
        private readonly FileBoxDbContext m_dbContext;
        private readonly IMapper m_mapper;

        public FolderService(FolderRepositoryDal folderDal, IMapper mapper, FileRepositoryDal fileRepositoryDal, FileBoxDbContext dbContext)
        {
            m_folderDal = folderDal;
            m_mapper = mapper;
            m_fileRepositoryDal = fileRepositoryDal;
            m_dbContext = dbContext;
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
        public async Task<(string folderPath, long folderId)> CreateFolder(FolderSaveDTO folderSaveDto)
        {
            var userUID = Guid.Parse(folderSaveDto.userId); // user uuid
            var folder = await m_folderDal.FindByIdAsync(folderSaveDto.currentFolderId);
            var folderNameWithoutPath = folderSaveDto.newFolderName; // just folder name


            var parentFolderPath = Util.DIRECTORY_BASE + folder.FolderPath;

            var userFolderPath = folder.FolderPath + "\\" + folderSaveDto.newFolderName;

            var fullName = parentFolderPath + "\\" + folderSaveDto.newFolderName;

            var fileBoxFolder = new FileboxFolder(GetParentFolderId(folder.FolderPath), userUID, folderNameWithoutPath, userFolderPath);

            await m_folderDal.Save(fileBoxFolder);
            await m_folderDal.SaveChangesAsync();

            if (!Directory.Exists(fullName))
                Directory.CreateDirectory(fullName);

            return (fileBoxFolder.FolderPath, fileBoxFolder.FolderId);
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
            var dir = await m_folderDal.FindByIdAsync(folderId); // given folder 

            CheckFolderAndPermits(dir, userID);

            var deletedFolderWithSubFolders = await GetAllSubFolders(dir); // subfolders

            // Remove all subfiles from db
            var context = new FileBoxDbContext();

            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (var folder in deletedFolderWithSubFolders)
                {
                    var files = await m_fileRepositoryDal.FindFilesByFolderId(folder.FolderId);
                    foreach (var file in files)
                        context.FileboxFiles.Remove(file);
                }

                context.FileboxFolders.RemoveRange(deletedFolderWithSubFolders);
                await context.SaveChangesAsync();
                transaction.Commit();

            }
            Directory.Delete(Path.Combine(Util.DIRECTORY_BASE, dir.FolderPath), true);         

            return dir.FolderPath;
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
        public async Task<FolderViewDto> RenameFolder(long folderId, string newFolderName, Guid userId)
        {
            try
            {
                var folder = await m_folderDal.FindByIdAsync(folderId); // find folder by id

                CheckFolderAndPermits(folder, userId);

                if (folder.ParentFolderId is null)
                    throw new ServiceException("Root folder name cannot be changed!");

                var oldFolderPathStartsWithRoot = folder.FolderPath; //"nuricanozturk\\Dotnet\\Dotnet"
                var oldFolderName = folder.FolderName; //"Dotnet"

                
                var oldFolderPathWithoutOldName = Path.GetDirectoryName(folder.FolderPath); // "nuricanozturk\\Dotnet"
                var newFolderPathStartsWithRoot = Path.Combine(oldFolderPathWithoutOldName, newFolderName); // "nuricanozturk\\Dotnet\\ASD"


                // Rename folder
                var oldFullPath = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath);
                var newFullPath = Path.Combine(Util.DIRECTORY_BASE, newFolderPathStartsWithRoot);

                await MoveFilesToTarget(folder, oldFullPath, newFullPath);
                
                // Remove old directory after copy operation
                Directory.Delete(oldFullPath, true);

                // Update parent folder
                folder.FolderName = newFolderName;
                folder.FolderPath = newFolderPathStartsWithRoot;
                folder.UpdatedDate = DateTime.Now;

                // Rename on db
                var context = new FileBoxDbContext();

                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.FileboxFolders.Update(folder);

                        var newFullPathOnSystem = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath);

                        // Update files of parent folder
                        var files = (await m_fileRepositoryDal.FindAllAsync()).Where(file => file.FilePath.Contains(oldFolderPathStartsWithRoot)).ToList();

                        files.ForEach(file => file.FilePath = file.FilePath.Replace(oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot));

                        context.FileboxFiles.UpdateRange(files);

                        var folders = (await m_folderDal.FindAllAsync()).Where(f => f.FolderPath.Contains(oldFolderPathStartsWithRoot)).ToList();

                        folders.ForEach(f => f.FolderPath = f.FolderPath.Replace(oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot));

                        context.FileboxFolders.UpdateRange(folders);


                        await context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                    }

                }
                   
                return m_mapper.Map<FolderViewDto>(folder);
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
         * Move files to targetPath. But with copy operation.
         * 
         */
        private async Task MoveFilesToTarget(FileboxFolder folder, string sourceFullPath, string targetFullPath)
        {            
            // Create Folder If not exists
            if (!Directory.Exists(targetFullPath))
                Directory.CreateDirectory(targetFullPath);
                        
            // Find SubFolders
            var subFolders = (await GetAllSubFolders(folder)).Where(sf => sf.FolderId != folder.FolderId);

            // Create the subfolders
            foreach(var subfolder in subFolders.Reverse())
            {
                var oldSubFolderPath = Path.Combine(Util.DIRECTORY_BASE, subfolder.FolderPath);
                var newSubFolderPath = oldSubFolderPath.Replace(sourceFullPath,targetFullPath);

                if (!Directory.Exists(newSubFolderPath))
                    Directory.CreateDirectory(newSubFolderPath);
            }
            // Copy subfiles to subfolders
            await CopySubFilesToSubFolders(subFolders, sourceFullPath, targetFullPath);
            await CopyFiles(folder, targetFullPath);
        }





        /*
         * 
         * Copy Files to target path but only mainfolder files 
         * 
         */
        private async Task CopyFiles(FileboxFolder folder, string targetFullPath)
        {
            var mainFiles = await m_fileRepositoryDal.FindFilesByFolderId(folder.FolderId); // Find files in main folder

            // Copy files to target main file           
            foreach (var file in mainFiles)
            {
                var oldFilePath = Path.Combine(Util.DIRECTORY_BASE, file.FilePath);
                var newFilePath = Path.Combine(targetFullPath, file.FileName);

                File.Copy(oldFilePath, newFilePath, true);
            }
        }





        /*
         * 
         * Copy subfiles to subfolders
         * 
         */
        private async Task CopySubFilesToSubFolders(IEnumerable<FileboxFolder> subFolders, string sourceFullPath, string targetFullPath)
        {
            foreach(var subfolder in subFolders)
            {
                // Find files on subfolder
                var filesOnSubFolders = await m_fileRepositoryDal.FindFilesByFolderId(subfolder.FolderId);
                
                var oldSubFolderPath = Path.Combine(Util.DIRECTORY_BASE, subfolder.FolderPath);
                var newSubFolderPath = oldSubFolderPath.Replace(sourceFullPath, targetFullPath);

                foreach (var file in filesOnSubFolders.Reverse())
                {
                    var oldFilePath = Path.Combine(oldSubFolderPath, file.FileName);
                    var newFilePath = oldFilePath.Replace(oldSubFolderPath, newSubFolderPath);

                    File.Copy(oldFilePath, newFilePath, true);
                }
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





        /*
         * 
         * Find all Folder and files with given user id
         * 
         */
        public async Task<IEnumerable<FoldersWithFilesDto>> FindFolderWithFiles(Guid guid)
        {
            var folders = await m_folderDal.FindFoldersByUserId(guid); // IEnumerableFileboxFolder
            
            var folderWithFiles = new List<FoldersWithFilesDto>();

            foreach (var folder in folders)
            {
               
                var files = await m_fileRepositoryDal.FindFilesByFolderId(folder.FolderId); // files on folder

                var dto = new FoldersWithFilesDto(folder.FolderName, folder.FolderPath, folder.CreationDate, folder.FolderId, folder.UserId.ToString(), folder.ParentFolderId, files.Select(f => new FileViewDto(f.FileId, f.FileName, f.FileType, f.FileSize, f.FilePath, f.CreatedDate, f.UpdatedDate)).ToList());

                folderWithFiles.Add(dto);
            }

            return folderWithFiles;
            
        }





        /*
         * 
         * Find all Folder and files with given user id and folder id
         * 
         */
        public async Task<IEnumerable<FoldersWithFilesDto>> FindFolderWithFiles(Guid guid, long folderId)
        {
            var folderRoot = await m_folderDal.FindByIdAsync(folderId);

            var folders = (await m_folderDal.FindByFilterAsync(f => f.ParentFolderId == folderId)).ToList();

            //folders.Add(folderRoot);
            
            var folderWithFiles = new List<FoldersWithFilesDto>();

            foreach (var folder in folders)
            {

                var files = await m_fileRepositoryDal.FindFilesByFolderId(folder.FolderId); // files on folder

                var dto = new FoldersWithFilesDto(folder.FolderName, folder.FolderPath, folder.CreationDate, folder.FolderId, folder.UserId.ToString(), folder.ParentFolderId, files.Select(f => new FileViewDto(f.FileId, f.FileName, f.FileType, f.FileSize, f.FilePath, f.CreatedDate, f.UpdatedDate)).ToList());

                folderWithFiles.Add(dto);
            }

            return folderWithFiles;

        }






        /*
         * 
         * Find Folder with folder id and user id
         * 
         */
        public async Task<FolderViewDto> FindFolderWithFolderId(Guid guid, long folderId)
        {
            var folder = (await m_folderDal.FindByFilterAsync(f => f.FolderId == folderId)).FirstOrDefault();

            if (folder == null)
                throw new ServiceException("Folder NOT FOUND!");

            CheckFolderAndPermits(folder, guid);

            return m_mapper.Map<FolderViewDto>(folder);
        }
    }
}