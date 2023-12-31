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

        public LoginController(IUserLoginService userLoginService)
        {
            m_userLoginService = userLoginService;
        }

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
                var userIdAndToken = await m_userLoginService.Login(userLoginDTO);

                return Ok(new ResponseMessage(true, "login operation is successful!", new
                {
                    username = userLoginDTO.Username,
                    user_id = userIdAndToken.uid,
                    token = userIdAndToken.token
                }));
            }   
            catch (ServiceException ex)
            {
                return Unauthorized(new ResponseMessage(false, ex.GetMessage, null));
            }

        }




        /*
         * 
         * 
         * Find User with given reset password token
         * 
         */     
        [HttpGet("find/user/token")]
        public async Task<IActionResult> FindUserByResetPasswordToken([FromQuery(Name = "token")] string token)
        {

            try
            {
                var user = await m_userLoginService.FindUserByResetPasswordToken(token);
               
                return Ok(new ResponseMessage(true, "login operation is successful!", new
                {
                    username = user.Username,
                    token = user.ResetPasswordToken
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(404, new ResponseMessage(false, ex.GetMessage, null));
            }
           
        }


        /*
         * 
         * 
         * User logout operation 
         * 
         */
        [Authorize]
        [HttpPost("logout/user")]
        public IActionResult Logout([FromQuery(Name = "uname")] string username, [FromQuery(Name = "token")] string token)
        {
            try
            {
                var response = m_userLoginService.Logout(username, token);
                return Ok(new ResponseMessage(true, "logout operation is successful!", response));
            }
            catch(ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }          
        }
    }
}
