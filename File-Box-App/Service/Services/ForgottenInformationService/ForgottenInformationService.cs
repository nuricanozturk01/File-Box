using RepositoryLib.Dal;
using Service.Exceptions;
using Service.Services.EmailService;
using Service.Services.TokenService;

namespace Service.Services.ForgottenInformationService
{
    public class ForgottenInformationService : IForgottenInformationService
    {
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly ITokenService m_tokenService;
        private readonly IEmailService m_emailService;

        private readonly string FORGOT_PASSWORD_LINK = "http://localhost:3000/reset-password-request?token={0}";


        public ForgottenInformationService(UserRepositoryDal userRepositoryDal, IEmailService emailService, ITokenService tokenService)
        {
            m_userRepositoryDal = userRepositoryDal;
            m_emailService = emailService;
            m_tokenService = tokenService;
        }





        /*
         * 
         * Send email reset password link to user
         * 
         */
        public async Task<(string email, string username, string token)> SendEmailForChangePassword(string email)
        {
            var user = await m_userRepositoryDal.FindUserByEmailAsync(email);

            if (user is null)
                throw new ServiceException("User not found!");

            var token = m_tokenService.CreateToken(user.UserId.ToString());

            await m_emailService.SendEmailAsync(email, "New Password", string.Format(FORGOT_PASSWORD_LINK, token));

            user.ResetPasswordToken = token;

            m_userRepositoryDal.Update(user);

            return (user.Email, user.Username, token);
        }





        /*
         * 
         * Validate the token. 
         * 
         */
        public async Task<bool> ValidateToken(string token)
        {
            var userId = m_tokenService.GetUserIdByToken(token);
            var user = await m_userRepositoryDal.FindByIdAsyncUser(Guid.Parse(userId));

            if (user.ResetPasswordToken is null || m_tokenService.IsTimeoutToken(token))
                throw new ServiceException("Request Timeout!");

            if (user.ResetPasswordToken != token)
                throw new ServiceException("Invalid Request!");

            return true;
        }





        /*
         * 
         *  Change User Password
         * 
         */
        public async Task<(string email, string username)> ChangePassword(string token, string newPassword)
        {
            var userId = m_tokenService.GetUserIdByToken(token);

            var user = await m_userRepositoryDal.FindByIdAsyncUser(Guid.Parse(userId));

            if (user is null)
                throw new ServiceException("User not found!");

            user.Password = Util.HashPassword(newPassword);

            m_userRepositoryDal.Update(user);            

            return (user.Email, user.Username);
        }
    }
}
