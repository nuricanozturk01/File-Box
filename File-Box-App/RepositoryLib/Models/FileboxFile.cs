namespace RepositoryLib.Models;

public partial class FileboxFile
{
    public long FileId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public string FilePath { get; set; } = null!;

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public long FolderId { get; set; }

    public virtual FileboxFolder Folder { get; set; } = null!;


    public FileboxFile() { }

    public FileboxFile(long folderId, string fileName, string filePath, string fileType, long fileSize)
    {
        FolderId = folderId;
        FileName = fileName;
        FileType = fileType;
        FileSize = fileSize;
        FilePath = filePath;
        CreatedDate = DateTime.Now;
        UpdatedDate = DateTime.Now;
    }


    public FileboxFile(long fileId, long folderId, string fileName, string filePath, string fileType, long fileSize)
    {
        FileId = fileId;
        FolderId = folderId;
        FileName = fileName;
        FileType = fileType;
        FileSize = fileSize;
        FilePath = filePath;
        CreatedDate = DateTime.Now;
        UpdatedDate = DateTime.Now;
    }
}
