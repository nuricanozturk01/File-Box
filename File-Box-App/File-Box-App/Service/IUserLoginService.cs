using File_Box_App.DTO;
using RepositoryLib.Models;

namespace File_Box_App.Service
{
    public interface IUserLoginService
    {
        bool Login(UserLoginDTO userLoginDTO);
        bool Logout(string username);

        Task<IEnumerable<FileboxFile>> FindAllFilesAsync();
        void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath);
        public FileboxFolder Save(FileboxFolder t);
    }
}
