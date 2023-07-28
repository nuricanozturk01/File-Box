using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Services.FolderService;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/folder")]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService m_folderService;

        public FolderController(IFolderService folderService)
        {
            m_folderService = folderService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateFolder([FromQuery(Name = "n")] string folderName, [FromBody] FolderViewDto? folderViewDto)
        {
            try
            {
                await m_folderService.CreateFolder(new FolderSaveDTO(folderName), folderViewDto);

                return Ok(folderName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("find/all/id")]
        public async Task<IActionResult> FindFoldersByUserId([FromQuery(Name = "id")] Guid id)
        {
            return Ok(await m_folderService.GetFoldersByUserIdAsync(id));
        }



        [HttpDelete("remove/dir")]
        public async Task<IActionResult> DeleteDirectory([FromQuery(Name = "id")] long folderId)
        {
            return Ok(await m_folderService.DeleteFolder(folderId));

        }
    }
}
