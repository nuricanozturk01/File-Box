using RepositoryLib.Models;

namespace Service.Services.ForgottenInformationService
{
    public interface IForgottenInformationService
    {


        /*
         * 
         * Send email reset password link to user
         * 
         */
        Task<(string email, string username)> SendEmailForChangePassword(string email);





        /*
         * 
         * Validate the token. 
         * 
         */
        Task<bool> ValidateToken(string token);





        /*
         * 
         *  Change User Password
         * 
         */
        Task<(string email, string username)> ChangePassword(string userId, string newPassword);
    }
}
