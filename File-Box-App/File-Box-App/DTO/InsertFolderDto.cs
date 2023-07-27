using RepositoryLib.Models;

namespace File_Box_App.DTO
{
    public class InsertFolderDto
    {
        public long FolderId { get; set; }

        public long? ParentFolderId { get; set; }

        public Guid UserId { get; set; }

        public string FolderName { get; set; } = null!;

        public DateTime? CreationDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public string FolderPath { get; set; } = null!;

        public virtual ICollection<FileboxFile> FileboxFiles { get; set; } = new List<FileboxFile>();

        public virtual ICollection<FileboxFolder> InverseParentFolder { get; set; } = new List<FileboxFolder>();

        public virtual FileboxFolder? ParentFolder { get; set; }

        public virtual FileboxUser User { get; set; } = null!;

       public InsertFolderDto(long folderId, long? parentFolderId, Guid userId, string folderName, DateTime? creationDate, DateTime? updatedDate, string folderPath, ICollection<FileboxFile> fileboxFiles, ICollection<FileboxFolder> inverseParentFolder, FileboxFolder? parentFolder, FileboxUser user)
        {
            FolderId = folderId;
            ParentFolderId = parentFolderId;
            UserId = userId;
            FolderName = folderName;
            CreationDate = creationDate;
            UpdatedDate = updatedDate;
            FolderPath = folderPath;
            FileboxFiles = fileboxFiles;
            InverseParentFolder = inverseParentFolder;
            ParentFolder = parentFolder;
            User = user;
        }
    }
}
