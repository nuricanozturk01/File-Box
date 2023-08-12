using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public record FolderCreatedResponseDto(string folder_path, long folderId, string folder_name);
}
