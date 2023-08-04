using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Services.ScanService;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/scan")]
    public class ScanController : ControllerBase
    {
        private readonly IScanService m_scanService;

        public ScanController(IScanService scanService)
        {
            m_scanService = scanService;
        }






        [HttpPost("uid")]
        public async Task<IActionResult> ScanUserForAllFolders([FromQuery(Name = "id")] string userId)
        {
            var scanOperation = await m_scanService.ScanUserAllFolders(Guid.Parse(userId));

            return Ok(scanOperation);
        }
    }
}
