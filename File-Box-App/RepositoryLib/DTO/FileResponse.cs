using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RepositoryLib.DTO
{
    public record FileResponseSuccessWithFileNameAndOwner ([property: JsonPropertyName("file_name")] string fileName, 
                                       [property: JsonPropertyName("file_owner")] string owner);
    public record FileDeleteSuccessResponse([property: JsonPropertyName("deleted_file_name")] string fileName);

    public record FileResponseSuccessWithFileName([property: JsonPropertyName("file_name")] string file_name);

    public record FileResponseSuccessRename([property: JsonPropertyName("old_file_name")] string oldFileName, [property: JsonPropertyName("new_file_name")] string newFileName);

    public record FileResponseFileList([property: JsonPropertyName("files")]  IEnumerable<FileViewDto> files);
}
