using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FolderViewDto
    {
        public string folderName { get; set; }

        public string folderPath { get; set; }

        public DateTime creationDate { get; set; }

        public long folderId { get; set; }

        public string userId { get; set; }

        public long? parentFolderId { get; set; }

        public FolderViewDto(string folderName, string folderPath, DateTime creationDate, long folderId, string userId, long? parentFolderId)
        {
            this.folderName = folderName;
            this.folderPath = folderPath;
            this.creationDate = creationDate;
            this.folderId = folderId;
            this.userId = userId;
            this.parentFolderId = parentFolderId;
        }
    }
}
