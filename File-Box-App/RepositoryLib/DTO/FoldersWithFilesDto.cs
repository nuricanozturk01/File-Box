using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FoldersWithFilesDto
    {
        public string folderName { get; set; }

        public string folderPath { get; set; }

        public DateTime? creationDate { get; set; }

        public long folderId { get; set; }

        [JsonIgnore]
        public string userId { get; set; }

        [JsonIgnore]
        public long? parentFolderId { get; set; }

        [JsonPropertyName("folder_files")]
        public List<FileViewDto> files { get; set; }

        public FoldersWithFilesDto(string folderName, string folderPath, DateTime? creationDate, long folderId, 
                                   string userId, long? parentFolderId, List<FileViewDto> files)
        {
            this.folderName = folderName;
            this.folderPath = folderPath;
            this.creationDate = creationDate;
            this.folderId = folderId;
            this.userId = userId;
            this.parentFolderId = parentFolderId;
            this.files = files;
        }
    }
}
