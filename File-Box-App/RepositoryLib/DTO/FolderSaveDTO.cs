using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FolderSaveDTO
    {
        [JsonPropertyName("new_folder_name")]
        [Required(ErrorMessage = "new folder name is required!")]
        public string newFolderName { get; set; }

        [JsonPropertyName("current_folder_id")]
        [Required(ErrorMessage = "current folder path is required!")]
        public long currentFolderId { get; set; }

        [JsonPropertyName("user_id")]
        [Required(ErrorMessage = "user id is required!")]
        public string userId { get; set; }

        public FolderSaveDTO() { }

        public FolderSaveDTO(string newFolderName, long currentFolderId, string userId)
        {
            this.newFolderName = newFolderName;
            this.currentFolderId = currentFolderId;
            this.userId = userId;
        }
    }
}
