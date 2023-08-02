using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public record FolderCreatedResponseDto(string folder_path, string folder_name);
}
