using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Exceptions;
using Service.Services.ForgottenInformationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([FromQuery(Name = "email")] string email)
        {
            try
            {
                var userInfo = await m_forgottenInformationService.ChangePasswordAsync(email);

                return Ok(new ResponseMessage(true, "Password is changed successfully!", new
                {
                    username = userInfo.username,
                    user_email = userInfo.email
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }
    }
}
