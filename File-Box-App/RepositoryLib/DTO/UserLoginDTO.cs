using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class UserLoginDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
