namespace RepositoryLib.DTO
{
    public class FileSaveDto
    {
        public string fileName { get; set; }
        public string fileType { get; set; }
        public long folderId { get; set; }
        public string userId { get; set; }

        public FileSaveDto(string fileName, string fileType, long folderId, string userId)
        {
            this.fileType = fileType;
            this.fileName = fileName;
            this.folderId = folderId;
            this.userId = userId;
        }
    }
}
