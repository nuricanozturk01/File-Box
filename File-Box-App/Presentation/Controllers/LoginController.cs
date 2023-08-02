using FileBoxService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Exceptions;
using RepositoryLib.DTO;

namespace Presentation.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserLoginService m_userLoginService;

        public LoginController(IUserLoginService userLoginService) => m_userLoginService = userLoginService;

        /*
         * 
         * User login operation 
         * 
         */
        [HttpPost("login/user")]
        public IActionResult Login([FromBody] UserLoginDTO userLoginDTO)
        {
            if (!m_userLoginService.Login(userLoginDTO))
                return Unauthorized(new ResponseMessage(false, "login operation is failed!", null));

            var tokenDto = m_userLoginService.CreateToken();
            
            return Ok(new ResponseMessage(true, "login operation is successful!", new UserSuccesfulLoginDto(userLoginDTO.Username, tokenDto)));
        }






        /*
         * 
         * 
         * User logout operation 
         * 
         */
        [Authorize]
        [HttpGet("/logout/user")]
        public IActionResult Logout([FromQuery(Name = "username")] string username)
        {
            return m_userLoginService.Logout(username) ?
                Ok(new MessageResponse("Success!", 200, $"{username} logout successfully!")) :
                BadRequest(new MessageResponse("Failure!", 500, $"logout operation is not successfull!"));
        }
    }
}
