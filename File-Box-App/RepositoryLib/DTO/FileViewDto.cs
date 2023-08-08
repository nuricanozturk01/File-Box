using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FileViewDto
    {
        [JsonPropertyName("file_name")]
        public string FileName { get; }
        
        [JsonPropertyName("file_type")]
        public string FileType { get; }
        
        [JsonPropertyName("file_byte")]
        public long FileSize { get; }
        
        [JsonPropertyName("file_path")]
        public string FilePath { get; }
        
        [JsonPropertyName("created_date")]
        public string CreatedDate { get; }
        
        [JsonPropertyName("updated_date")]
        public string UpdateDate { get; }

        [JsonPropertyName("real_path")]
        public string RealMachinePath { get; } //"C:\\Users\\hp\\Desktop\\file_box\\ahmetkoc\\files\\deneme.docx"

        public FileViewDto(string fileName, string fileType, long fileSize, string filePath, DateTime? createdDate, DateTime? updatedDate) 
        {
            FileName = fileName;
            FileType = fileType;
            FileSize = fileSize;
            FilePath = filePath;
            UpdateDate = updatedDate?.ToString("dd/MM/yyyy HH:mm:ss");
            CreatedDate = createdDate?.ToString("dd/MM/yyyy HH:mm:ss");
            RealMachinePath = @"C:\Users\hp\WebstormProjects\filebox\src\components\file_box\" + FilePath;
        }
    }
}
