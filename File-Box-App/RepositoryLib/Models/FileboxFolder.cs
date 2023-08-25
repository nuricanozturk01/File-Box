namespace RepositoryLib.Models;

public partial class FileboxFolder
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

    public FileboxFolder() { }

    public FileboxFolder(long? parentFolderId, Guid userId, string folderName, string folderPath)
    {
        ParentFolderId = parentFolderId;
        UserId = userId;
        FolderName = folderName;
        FolderPath = folderPath;
        CreationDate = DateTime.Now;
        UpdatedDate = DateTime.Now;
    }

    public FileboxFolder(Guid userId, string folderName, string folderPath)
    {
        UserId = userId;
        FolderName = folderName;
        FolderPath = folderPath;
        CreationDate = DateTime.Now;
        UpdatedDate = DateTime.Now;
    }
}
