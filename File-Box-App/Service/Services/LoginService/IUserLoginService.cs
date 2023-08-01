using RepositoryLib.DTO;

namespace FileBoxService.Service
{
    public interface IUserLoginService
    {






        /*
         * 
         * 
         * Login operation for user with given userLoginDto parameter. 
         * returns the status of login operation
         * 
         */
        bool Login(UserLoginDTO userLoginDTO);






        /*
         * 
         * 
         * Logout operation [NOT IMPLEMENTED YET]
         * 
         * 
         */
        bool Logout(string username);






        /*
         * 
         * 
         * Create jwt token and return it
         * 
         * 
         */
        string CreateToken();
    }
}
