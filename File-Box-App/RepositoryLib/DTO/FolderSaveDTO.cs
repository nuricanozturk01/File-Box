namespace RepositoryLib.DTO
{
    public class FolderSaveDTO
    {


        public string folderName { get; set; }

        public FolderSaveDTO() { }

        public FolderSaveDTO(string folderName)
        {
         

            this.folderName = folderName;

        }
    }
}
