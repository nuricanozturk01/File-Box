using System;
using System.Collections.Generic;

namespace File_Box_App.Repository.Models;

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
}
