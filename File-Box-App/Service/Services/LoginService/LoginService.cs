using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using Service;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace FileBoxService.Service
{
    public class LoginService : IUserLoginService
    {
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly IConfiguration m_configuration;

        public LoginService(UserRepositoryDal userRepositoryDal, IConfiguration configuration)
        {
            m_userRepositoryDal = userRepositoryDal;
            m_configuration = configuration;
        }

        public  string CreateToken()
        {
            var signinCredentials = GetSignInCredentials();

            var tokenOptions = GenerateTokenOptions(signinCredentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return accessToken;
        }

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

        private SigningCredentials GetSignInCredentials()
        {
            var jwtSettings = m_configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }


        public bool Logout(string username)
        {
            throw new NotImplementedException();
        }
        internal void CreateDirectoryIfNotExists(string username)
        {
            var dirName = Util.DIRECTORY_BASE + username;
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);
        }
        public bool Login(UserLoginDTO userLoginDTO)
        {
            var user = m_userRepositoryDal.FindByFilterUser(usr =>
                                                        usr.Username == userLoginDTO.Username &&
                                                        usr.Password == userLoginDTO.Password)
                                            .FirstOrDefault();
            if (user is null)
                return false;

            CreateDirectoryIfNotExists(user.Username);

            return true;
        }
    }
}
