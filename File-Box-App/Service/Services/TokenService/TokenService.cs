using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Service.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration m_configuration;

        public TokenService(IConfiguration configuration)
        {
            m_configuration = configuration;
        }






        /*
         * 
         * 
         * Create jwt token and return it
         * 
         * 
         */
        public string CreateToken(string userId)
        {
            var signinCredentials = GetSignInCredentials();

            var claimList = new List<Claim>
            {
                new Claim("userId", userId)
            };

            var tokenOptions = GenerateTokenOptions(signinCredentials, claimList);

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
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signinCredentials, List<Claim> claims)
        {
            var jwtSettings = m_configuration.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                    issuer: jwtSettings["validIssuer"],
                    audience: jwtSettings["validAudience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                    signingCredentials: signinCredentials);;

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
         * Find the userId claim by token. 
         * 
         */
        public string GetUserIdByToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSettings = m_configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["secretKey"]);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "userId");

                if (userIdClaim != null)
                    return userIdClaim.Value;

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }






        /*
         * 
         * 
         * Check Is token timeout 
         * 
         */
        public  bool IsTimeoutToken(string token)
        {
            var jwtSettings = m_configuration.GetSection("JwtSettings");
            var tokenTicks = jwtSettings["expires"];

            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenTicks)).UtcDateTime;

            var now = DateTime.Now.ToUniversalTime();

            var valid = tokenDate >= now;

            return valid;
        }
    }
}
