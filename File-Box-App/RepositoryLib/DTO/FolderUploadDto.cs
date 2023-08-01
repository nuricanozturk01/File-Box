using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryLib.DTO
{
    public class FolderUploadDto
    {
        [JsonPropertyName("source_file_name")]
        public string sourceFilePath {  get; set; }

        public FolderUploadDto(string sourceFilePath)
        {
            this.sourceFilePath = sourceFilePath;
        }
    }
}
