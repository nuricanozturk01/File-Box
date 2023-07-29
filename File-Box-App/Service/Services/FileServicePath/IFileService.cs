﻿using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace Service.Services.FileServicePath
{
    public interface IFileService
    {
        Task<IEnumerable<FileboxFile>?> GetFilesByUserIdAsync(Guid userId, long folderId);
        Task<bool> CreateFile(FileSaveDto fileSaveDto);
        Task<bool> DeleteFile(long folderId);
        void RenameFile(long fileId, string newFileName);
    }
}
