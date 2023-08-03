using RepositoryLib.Dal;
using Service.Exceptions;
using Service.Services.EmailService;
using Service.Services.PasswordGenerator;

namespace Service.Services.ForgottenInformationService
{
    public class ForgottenInformationService : IForgottenInformationService
    {
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly IPasswordGenerator m_passwordGenerator;
        private readonly IEmailService m_emailService;


        public ForgottenInformationService(UserRepositoryDal userRepositoryDal, IPasswordGenerator passwordGenerator, IEmailService emailService)
        {
            m_userRepositoryDal = userRepositoryDal;
            m_passwordGenerator = passwordGenerator;
            m_emailService = emailService;
        }


        public async Task<(string email, string username)> ChangePasswordAsync(string email)
        {
            var user = await m_userRepositoryDal.FindUserByEmailAsync(email);

            if (user is null)
                throw new ServiceException("User not found!");

            var newPassword = m_passwordGenerator.Generate();

            await m_emailService.SendEmailAsync(email, "New Password", newPassword);

            user.Password = Util.HashPassword(newPassword);

            m_userRepositoryDal.Update(user);
            m_userRepositoryDal.SaveChanges();

            return (user.Email, user.Username);
        }
    }
}
