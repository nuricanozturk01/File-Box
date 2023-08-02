using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryLib.DTO
{
    public record UploadSingleFileResponse([property: JsonPropertyName("target_path")] string path, [property: JsonPropertyName("file_length")] string length);
    public record UploadFolderResponse([property: JsonPropertyName("target_path")] List<string> pathList);
}
