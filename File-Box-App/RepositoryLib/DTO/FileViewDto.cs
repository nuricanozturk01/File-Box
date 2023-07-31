using System.Text.Json.Serialization;

namespace RepositoryLib.DTO
{
    public class FileViewDto
    {
        public string FileName { get; }
        public string FileType { get; }
        public long FileSize { get; }
        public string FilePath { get; }
        public string CreatedDate { get; }
        public string UpdateDate { get; }

        [JsonIgnore]
        public string RealMachinePath { get; } //"C:\\Users\\hp\\Desktop\\file_box\\ahmetkoc\\files\\deneme.docx"

        public FileViewDto(string fileName, string fileType, long fileSize, string filePath, DateTime createdDate, DateTime updatedDate) 
        {
            FileName = fileName;
            FileType = fileType;
            FileSize = fileSize;
            FilePath = filePath;
            UpdateDate = updatedDate.ToString("dd/MM/yyyy HH:mm:ss");
            CreatedDate = createdDate.ToString("dd/MM/yyyy HH:mm:ss");
            RealMachinePath = @"C:\Users\hp\Desktop\file_box\" + FilePath;
        }
    }
}
