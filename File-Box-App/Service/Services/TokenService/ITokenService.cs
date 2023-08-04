namespace Service.Services.TokenService
{
    public interface ITokenService
    {
        /*
         * 
         * 
         * Create jwt token and return it
         * 
         * 
         */
        string CreateToken(string userId);





        
        /*
         * 
         * Find the userId claim by token. 
         * 
         */
        string GetUserIdByToken(string token);






        /*
         * 
         * 
         * Check Is token timeout 
         * 
         */
        bool IsTimeoutToken(string token);
    }
}
