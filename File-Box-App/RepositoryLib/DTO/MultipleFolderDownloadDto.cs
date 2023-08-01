using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class MultipleFolderDownloadDto
    {
        [JsonPropertyName("folder_id")]
        public long folderId { get; set; }
    }
}
