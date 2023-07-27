using File_Box_App.DTO;
using File_Box_App.Exceptions;
using File_Box_App.Service;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.Models;

namespace File_Box_App.Controllers
{
    [Route("users")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserLoginService m_userLoginService;

        public LoginController(IUserLoginService userLoginService) => m_userLoginService = userLoginService;

        [HttpPost("folder")]
        public void InsertFolderTry([FromBody] FileboxFolder dto)
        {
            m_userLoginService.Save(dto);
        }

        [HttpPost("login/user")]
        public IActionResult Login([FromBody] UserLoginDTO userLoginDTO)
        {
            return m_userLoginService.Login(userLoginDTO) ?
                  Ok(new MessageResponse("Success!", 200, $"{userLoginDTO.Username} login operation is successful!")) :
                  BadRequest(new MessageResponse("Login Failure!", 404, "username or password is wrong!"));
        }

        [HttpGet("/logout/user")]
        public IActionResult Logout([FromQuery(Name = "username")] string username)
        {
            return m_userLoginService.Logout(username) ?
                Ok(new MessageResponse("Success!", 200, $"{username} logout successfully!")) :
                BadRequest(new MessageResponse("Failure!", 500, $"logout operation is not successfull!"));
        }
    }
}
