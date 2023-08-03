using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using Service;
using Service.Exceptions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace FileBoxService.Service
{
    public class LoginService : IUserLoginService
    {
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly FolderRepositoryDal m_folderRepositoryDal;
        private readonly IConfiguration m_configuration;


        public LoginService(UserRepositoryDal userRepositoryDal, IConfiguration configuration, FolderRepositoryDal folderRepositoryDal)
        {
            m_userRepositoryDal = userRepositoryDal;
            m_configuration = configuration;
            m_folderRepositoryDal = folderRepositoryDal;
        }






        /*
         * 
         * 
         * Create jwt token and return it
         * 
         * 
         */
        public string CreateToken()
        {
            var signinCredentials = GetSignInCredentials();

            var tokenOptions = GenerateTokenOptions(signinCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return accessToken;
        }






        /*
         * 
         * 
         * Control and validate the jwt token options
         * returns JwtSecurityToken
         * 
         */
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials)
        {
            var jwtSettings = m_configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                    issuer: jwtSettings["validIssuer"],
                    audience: jwtSettings["validAudience"],
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                    signingCredentials: signinCredentials);

            return tokenOptions;
        }






        /*
         * 
         * 
         * Return the SigningCredentials about jwt tokents.
         * 
         * 
         */
        private SigningCredentials GetSignInCredentials()
        {
            var jwtSettings = m_configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }






        /*
         * 
         * 
         * Logout operation [NOT IMPLEMENTED YET]
         * 
         * 
         */
        public bool Logout(string username)
        {
            throw new NotImplementedException();
        }






        /*
         * 
         * 
         * If login operation is successfull, create the root file for user
         * 
         * 
         */
        internal async void CreateDirectoryIfNotExists(string username, Guid userId, FileboxUser user)
        {
            var dirName = Util.DIRECTORY_BASE + username;

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
                user.FileboxFolders.Add(new FileboxFolder(null, userId, username, username));
                m_userRepositoryDal.Update(user);
                m_userRepositoryDal.SaveChanges();
            }
            else
            {
                var rootFolder = (await m_folderRepositoryDal.FindByFilterAsync(folder => folder.UserId == user.UserId)).FirstOrDefault();

                if (rootFolder is null)
                {
                    await m_folderRepositoryDal.Save(new FileboxFolder(null, userId, username, username));
                    await m_folderRepositoryDal.SaveChangesAsync();
                }
            }
        }






        /*
         * 
         * 
         * Login operation for user with given userLoginDto parameter. 
         * returns the status of login operation
         * 
         */
        public async Task Login(UserLoginDTO userLoginDTO)
        {
            var user = (await m_userRepositoryDal.FindByFilterAsyncUser(user => user.Username == userLoginDTO.Username)).FirstOrDefault();

            if (user is null || !Util.VerifyPassword(userLoginDTO.Password, user.Password))
                throw new ServiceException("Username or password is invalid!");

            CreateDirectoryIfNotExists(user.Username, user.UserId, user);
            
        }





        /*
         * 
         * 
         * Write Last token to database 
         * 
         * 
         */
        public async void WriteTokenToDb(string tokenDto, string username)
        {
            if (!string.IsNullOrEmpty(tokenDto))
            {
                var user = await m_userRepositoryDal.FindUserByUsername(username);

                user.LastToken = tokenDto;

                m_userRepositoryDal.Update(user);

                await m_userRepositoryDal.SaveChangesAsync();
            }
        }
    }
}
