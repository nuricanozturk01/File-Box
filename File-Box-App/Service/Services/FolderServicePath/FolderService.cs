using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using System.Data;

namespace Service.Services.FolderService
{
    public class FolderService : IFolderService
    {
        private readonly FolderRepositoryDal m_folderDal;
        private readonly FileRepositoryDal m_fileRepositoryDal;
        private readonly IMapper m_mapper;
        private readonly UnitOfWork m_unitOfWork;
        public FolderService(FolderRepositoryDal folderDal, IMapper mapper, FileRepositoryDal fileRepositoryDal, UnitOfWork unitOfWork)
        {
            m_folderDal = folderDal;
            m_mapper = mapper;
            m_fileRepositoryDal = fileRepositoryDal;
            m_unitOfWork = unitOfWork;
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
        public async Task<(string folderPath, long folderId, string creationDate)> CreateFolder(FolderSaveDTO folderSaveDto)
        {
            try
            {
                var userUID = Guid.Parse(folderSaveDto.userId); // user uuid

                var folder = await m_unitOfWork.FolderRepository.FindByIdAsync(folderSaveDto.currentFolderId);

                if (folder.UserId != userUID)
                    throw new ServiceException("Folder owner not this user!");

                var folderNameWithoutPath = Util.ConvertToEnglishCharacters(folderSaveDto.newFolderName); // just folder name

                var parentFolderPath = Util.DIRECTORY_BASE + folder.FolderPath;
                var userFolderPath = folder.FolderPath + "\\" + folderSaveDto.newFolderName;
                var fullName = parentFolderPath + "\\" + folderSaveDto.newFolderName;

                var newFolder = new FileboxFolder(GetParentFolderId(folder.FolderPath), userUID, folderNameWithoutPath, userFolderPath);

                await m_unitOfWork.FolderRepository.SaveAsync(newFolder);
                m_unitOfWork.Save();

                if (!Directory.Exists(fullName))
                    Directory.CreateDirectory(fullName);

                return (newFolder.FolderPath, newFolder.FolderId, newFolder.CreationDate?.ToString("dd/MM/yyyy HH:mm:ss"));
            }
            catch
            {
                throw;
            }
     
        }








        /*
         * 
         * 
         * Find all subfolders given folder and sort it by folderPath length.
         * 
         * 
         */
        private async Task<IEnumerable<FileboxFolder>> GetAllSubFolders(UnitOfWork unitOfWork, FileboxFolder folder)
        {
            var folders = await unitOfWork.FolderRepository.FindByFilterAsync(f => f.FolderId >= folder.FolderId && f.FolderPath.Contains(folder.FolderName));

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
            try
            {
                m_unitOfWork.Context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTrackingWithIdentityResolution;

                var folder = await m_unitOfWork.FolderRepository.FindByIdAsync(folderId);

                CheckFolderAndPermits(folder, userID);

                var deletedFolderWithSubFolders = await GetAllSubFolders(m_unitOfWork, folder);

                foreach (var folderToDelete in deletedFolderWithSubFolders)
                {
                    var files = await m_unitOfWork.FileRepository.FindByFilterAsync(f => f.FolderId == folderToDelete.FolderId);
                    foreach (var file in files)
                    {
                        await m_unitOfWork.FileRepository.Delete(file);
                    }

                    await m_unitOfWork.FolderRepository.Delete(folderToDelete);
                }

                m_unitOfWork.Save();

                Directory.Delete(Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath), true);

                return folder.FolderPath;
            }
            catch
            {
                throw;
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
        public async Task<FolderViewDto> RenameFolder(long folderId, string newFolderName, Guid userId)
        {

            try
            {
                var folder = await m_unitOfWork.FolderRepository.FindByIdAsync(folderId);
                CheckFolderAndPermits(folder, userId);

                if (folder.ParentFolderId == null)
                    throw new ServiceException("Root folder name cannot be changed!");

                var oldFolderPathStartsWithRoot = folder.FolderPath;
                var oldFolderName = folder.FolderName;
                var oldFolderPathWithoutOldName = Path.GetDirectoryName(folder.FolderPath);
                var newFolderPathStartsWithRoot = Path.Combine(oldFolderPathWithoutOldName, newFolderName);

                var oldFullPath = Path.Combine(Util.DIRECTORY_BASE, folder.FolderPath);
                var newFullPath = Path.Combine(Util.DIRECTORY_BASE, newFolderPathStartsWithRoot);

                await MoveFilesToTarget(m_unitOfWork, folder, oldFullPath, newFullPath);

                Directory.Delete(oldFullPath, true);

                folder.FolderName = newFolderName;
                folder.FolderPath = newFolderPathStartsWithRoot;
                folder.UpdatedDate = DateTime.Now;

                m_unitOfWork.FolderRepository.Update(folder);

                var filesToUpdate = await m_unitOfWork.FileRepository.FindByFilterAsync(f => f.FilePath.Contains(oldFolderPathStartsWithRoot));

                foreach (var file in filesToUpdate)
                {
                    file.FilePath = file.FilePath.Replace(oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot);
                    m_unitOfWork.FileRepository.Update(file);
                }

                var foldersToUpdate = await m_unitOfWork.FolderRepository.FindByFilterAsync(f => f.FolderPath.Contains(oldFolderPathStartsWithRoot));

                foreach (var folderToUpdate in foldersToUpdate)
                {
                    folderToUpdate.FolderPath = folderToUpdate.FolderPath.Replace(oldFolderPathStartsWithRoot, newFolderPathStartsWithRoot);
                    m_unitOfWork.FolderRepository.Update(folderToUpdate);
                }



                m_unitOfWork.Save();

                return m_mapper.Map<FolderViewDto>(folder);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ServiceException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ServiceException(ex.Message);
            }


        }






        /*
         * 
         * Move files to targetPath. But with copy operation.
         * 
         */
        private async Task MoveFilesToTarget(UnitOfWork unitOfWork, FileboxFolder folder, string sourceFullPath, string targetFullPath)
        {
            // Create Folder If not exists
            if (!Directory.Exists(targetFullPath))
                Directory.CreateDirectory(targetFullPath);

            // Find SubFolders
            var subFolders = (await GetAllSubFolders(unitOfWork, folder)).Where(sf => sf.FolderId != folder.FolderId);

            // Create the subfolders
            foreach (var subfolder in subFolders.Reverse())
            {
                var oldSubFolderPath = Path.Combine(Util.DIRECTORY_BASE, subfolder.FolderPath);
                var newSubFolderPath = oldSubFolderPath.Replace(sourceFullPath, targetFullPath);

                if (!Directory.Exists(newSubFolderPath))
                    Directory.CreateDirectory(newSubFolderPath);
            }
            // Copy subfiles to subfolders
            await CopySubFilesToSubFolders(unitOfWork, subFolders, sourceFullPath, targetFullPath);
            await CopyFiles(unitOfWork, folder, targetFullPath);
        }





        /*
         * 
         * Copy Files to target path but only mainfolder files 
         * 
         */
        private async Task CopyFiles(UnitOfWork unitOfWork, FileboxFolder folder, string targetFullPath)
        {
            var mainFiles = await unitOfWork.FileRepository.FindByFilterAsync(f => f.FolderId == folder.FolderId); // Find files in main folder

            foreach (var file in mainFiles)
            {
                var oldFilePath = Path.Combine(Util.DIRECTORY_BASE, file.FilePath);
                var newFilePath = Path.Combine(targetFullPath, file.FileName);

                try
                {
                    // Ensure the target directory exists
                    Directory.CreateDirectory(targetFullPath);

                    // Open the source file
                    using (var sourceStream = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        // Open or create the destination file
                        using (var destinationStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            // Copy data from source to destination
                            await sourceStream.CopyToAsync(destinationStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error copying file: {ex.Message}");
                }
            }
        }






        /*
         * 
         * Copy subfiles to subfolders
         * 
         */
        private async Task CopySubFilesToSubFolders(UnitOfWork unitOfWork, IEnumerable<FileboxFolder> subFolders, string sourceFullPath, string targetFullPath)
        {
            foreach (var subfolder in subFolders)
            {
                try
                {
                    // Ensure the target subfolder exists
                    var oldSubFolderPath = Path.Combine(Util.DIRECTORY_BASE, subfolder.FolderPath);
                    var newSubFolderPath = oldSubFolderPath.Replace(sourceFullPath, targetFullPath);
                    Directory.CreateDirectory(newSubFolderPath);

                    // Find files in the subfolder
                    var filesOnSubFolders = await unitOfWork.FileRepository.FindByFilterAsync(f => f.FolderId == subfolder.FolderId);

                    foreach (var file in filesOnSubFolders.Reverse())
                    {
                        var oldFilePath = Path.Combine(oldSubFolderPath, file.FileName);
                        var newFilePath = Path.Combine(newSubFolderPath, file.FileName);

                        using (var sourceStream = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                        using (var destinationStream = new FileStream(newFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await sourceStream.CopyToAsync(destinationStream);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    Console.WriteLine($"Error copying subfolder files: {ex.Message}");
                    // You can log, rethrow, or handle the exception as appropriate for your application
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