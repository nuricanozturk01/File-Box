using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FolderUploadDto
    {
        [JsonPropertyName("source_file_name")]
        public string sourceFilePath { get; set; }

        public FolderUploadDto(string sourceFilePath)
        {
            this.sourceFilePath = sourceFilePath;
        }
    }
}
