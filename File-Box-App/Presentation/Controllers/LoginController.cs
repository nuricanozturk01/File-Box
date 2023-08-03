﻿using FileBoxService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repository.Exceptions;
using RepositoryLib.DTO;
using Service.Exceptions;

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
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userLoginDTO)
        {

            try
            {
                await m_userLoginService.Login(userLoginDTO);

                var tokenDto = m_userLoginService.CreateToken();

                m_userLoginService.WriteTokenToDb("Bearer " + tokenDto, userLoginDTO.Username);
                
                return Ok(new ResponseMessage(true, "login operation is successful!", new UserSuccesfulLoginDto(userLoginDTO.Username, tokenDto)));
            }
            catch (ServiceException ex)
            {
                return Unauthorized(new ResponseMessage(false, ex.GetMessage, null));
            }
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
