using RepositoryLib.DTO;

namespace FileBoxService.Service
{
    public interface IUserLoginService
    {
        bool Login(UserLoginDTO userLoginDTO);
        bool Logout(string username);
        string CreateToken();
    }
}
