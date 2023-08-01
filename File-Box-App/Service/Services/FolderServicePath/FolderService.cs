﻿using AutoMapper;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;

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
        public async Task<bool> DeleteFolder(long folderId)
        {
            var dir = await m_folderDal.FindByIdAsync(folderId);

            var deletedFolderWihtSubFolders = GetAllSubFolders(dir);

            m_folderDal.RemoveAll(deletedFolderWihtSubFolders);

            deletedFolderWihtSubFolders.ToList().ForEach(f => Directory.Delete(Util.DIRECTORY_BASE + f.FolderPath, true));

            return true;
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
        public async void RenameFolder(long folderId, string newFolderName)
        {
            var folder = await m_folderDal.FindByIdAsync(folderId); // find folder by id

            var oldPath = folder.FolderPath; //nuricanozturk/dev/nuri
            var oldName = folder.FolderName; //nuri

            // nuricanozturk/dev/nuri     new name: can
            var oldPathParent = Path.GetDirectoryName(folder.FolderPath); // without last folder (nuricanozturk/dev)
            var newFullPath = Path.Combine(oldPathParent, newFolderName); // nuricanozturk/dev/can

            Directory.Move(Util.DIRECTORY_BASE + folder.FolderPath, Util.DIRECTORY_BASE + newFullPath);

            folder.FolderName = newFolderName;
            folder.FolderPath = newFullPath;
            folder.UpdatedDate = DateTime.Now;

            var files = await m_fileRepositoryDal.FindByFilterAsync(file => file.FolderId == folderId);

            foreach (var file in files)
            {
                file.FilePath = file.FilePath.Replace(oldName, newFolderName);
                file.UpdatedDate = DateTime.Now;
            }
            await m_fileRepositoryDal.UpdateAll(files);


            var subfolders = await m_folderDal.FindByFilterAsync(f => f.FolderPath.Contains(oldPath));

            foreach (var subfolder in subfolders)
            {
                subfolder.UpdatedDate = DateTime.Now;
                subfolder.FolderPath = subfolder.FolderPath.Replace(oldName, newFolderName);
            }

            await m_folderDal.UpdateAll(subfolders);
        }
    }
}