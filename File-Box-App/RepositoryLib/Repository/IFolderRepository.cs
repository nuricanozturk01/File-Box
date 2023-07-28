﻿using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IFolderRepository : ICrudRepository<FileboxFolder, long>
    {
        void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath);
    }
}
