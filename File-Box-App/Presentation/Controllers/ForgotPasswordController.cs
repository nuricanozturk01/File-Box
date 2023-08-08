using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Exceptions;
using Service.Services.ForgottenInformationService;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/change")]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgottenInformationService m_forgottenInformationService;

        public ForgotPasswordController(IForgottenInformationService forgottenInformationService)
        {
            m_forgottenInformationService = forgottenInformationService;
        }





        /*
         * 
         * Send Reset Password link to email.
         * 
         */
        [HttpPost("password")]
        public async Task<IActionResult> SendEmailForChangePassword([FromQuery(Name = "email")] string email)
        {
            try
            {
                var userInfo = await m_forgottenInformationService.SendEmailForChangePassword(email);
                
                return Ok(new ResponseMessage(true, "Password Change link sent to email!", new
                {
                    username = userInfo.username,
                    user_email = userInfo.email,
                    user_token = userInfo.token
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }





        /*
         * 
         * Check token and user is valid and check reset password token is equal to given token.
         * 
         */
        [HttpGet("reset-request")]
        public async Task<IActionResult> CheckResetPasswordRequest([FromQuery(Name = "token")] string token)
        {
            try
            {
                var result = await m_forgottenInformationService.ValidateToken(token);

                return Ok(new ResponseMessage(true, "Success", result ? token : false));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }





        /*
         * 
         * Reset Password and update the user. 
         * 
         */
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromQuery(Name = "token")] string token, [FromQuery(Name = "p")] string newPassword)
        {
            try
            {
                var result = await m_forgottenInformationService.ChangePassword(token, newPassword);

                return Ok(new ResponseMessage(true, "Password Changed Successfully!", result));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }
    }
}
