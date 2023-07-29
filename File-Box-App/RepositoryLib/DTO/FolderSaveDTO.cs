namespace RepositoryLib.DTO
{
    public class FolderSaveDTO
    {
        public string newFolderName { get; set; }
        public string currentFolderPath { get; set; }
        public string userId { get; set; }

        public FolderSaveDTO() { }

        public FolderSaveDTO(string newFolderName, string currentFolderPath, string userId)
        {
            this.newFolderName = newFolderName;
            this.currentFolderPath = currentFolderPath;
            this.userId = userId;
        }
    }
}
