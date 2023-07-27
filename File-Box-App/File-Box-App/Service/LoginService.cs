using File_Box_App.DTO;
using RepositoryLib.Dal;
using RepositoryLib.Models;

namespace File_Box_App.Service
{
    public class LoginService : IUserLoginService
    {
        private readonly FileBoxDbContext m_dbContext;
        private readonly FileBoxAppDal m_fileBoxDal;

        public LoginService(FileBoxDbContext dbContext, FileBoxAppDal fileBoxDal)
        {
            m_fileBoxDal = fileBoxDal;
            m_dbContext = dbContext;
        }

        public bool Login(UserLoginDTO userLoginDTO)
        {
            var user = m_dbContext.FileboxUsers
                .Where(usr => usr.Username == userLoginDTO.Username && usr.Password == userLoginDTO.Password)
                .FirstOrDefault();

            if (user is null)
                return false;

            return true;
        }

        public bool Logout(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxFile>> FindAllFilesAsync()
        {
            return m_fileBoxDal.FindAllFilesAsync();
        }
        public FileboxFolder Save(FileboxFolder t)
        {
            return m_fileBoxDal.Save(t);
        }
        public void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath)
        {
            m_fileBoxDal.InsertFolder(parentFolder, userId, folderName, folderPath);
        }
    }
}
