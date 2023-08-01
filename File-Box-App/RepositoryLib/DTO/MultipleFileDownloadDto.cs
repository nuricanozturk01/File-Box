using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryLib.DTO
{
    public class MultipleFileDownloadDto
    {
        [JsonPropertyName("file_id")]
        public long fileId { get; set; }
    }
}
