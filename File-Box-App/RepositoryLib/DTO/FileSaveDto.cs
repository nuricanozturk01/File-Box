using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FileSaveDto
    {
        [JsonPropertyName("file_name")]
        public string fileName { get; set; }

        [JsonPropertyName("file_type")]
        public string fileType { get; set; }

        [JsonPropertyName("folder_id")]
        public long folderId { get; set; }

        [JsonPropertyName("user_id")]
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
