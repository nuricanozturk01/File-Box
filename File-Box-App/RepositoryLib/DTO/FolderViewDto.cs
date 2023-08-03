using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FolderViewDto
    {
        public string folderName { get; set; }

        public string folderPath { get; set; }

        public DateTime creationDate { get; set; }

        [JsonIgnore]
        public long folderId { get; set; }

        [JsonIgnore]
        public string userId { get; set; }

        [JsonIgnore]
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
