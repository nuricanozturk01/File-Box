using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class MultipleFileDownloadDto
    {
        [JsonPropertyName("file_id")]
        public long fileId { get; set; }
    }
}
