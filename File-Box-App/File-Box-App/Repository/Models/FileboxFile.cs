using System;
using System.Collections.Generic;

namespace File_Box_App.Repository.Models;

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
}
