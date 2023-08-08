using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace FileBoxService.Service
{
    public interface IUserLoginService
    {






        /*
         * 
         * 
         * Find User with given reset password token
         * 
         */
        Task<FileboxUser> FindUserByResetPasswordToken(string token);






        /*
         * 
         * 
         * Login operation for user with given userLoginDto parameter. 
         * returns the status of login operation
         * 
         */
        Task<(string? token, string uid)> Login(UserLoginDTO userLoginDTO);






        /*
         * 
         * 
         * Logout operation [NOT IMPLEMENTED YET]
         * 
         * 
         */
        bool Logout(string username);
    }
}
