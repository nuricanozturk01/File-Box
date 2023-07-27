using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepositoryLib.Models;

public partial class FileboxFolder
{
    
    public long FolderId { get; set; }
    [JsonPropertyName("parent_folder_id")]
    public long? ParentFolderId { get; set; }
    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; }
    [JsonPropertyName("folder_name")]
    public string FolderName { get; set; } = null!;

    public DateTime? CreationDate { get; set; }

    public DateTime? UpdatedDate { get; set; }
    [JsonPropertyName("folder_path")]
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
    }
}
