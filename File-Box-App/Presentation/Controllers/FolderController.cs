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







        /*
         * 
         * 
         * Create empty folder with given FolderSaveDto body parameter 
         * 
         */
        [HttpPost("create")]
        public async Task<IActionResult> CreateFolder([FromBody] FolderSaveDTO folderSaveDTO)
        {
            try
            {
                await m_folderService.CreateFolder(folderSaveDTO);

                return Ok("Created folder on " + folderSaveDTO.currentFolderPath + "\\" + folderSaveDTO.newFolderName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }






        /*
         * 
         * 
         * Find Folders given user id
         * 
         */
        [HttpGet("find/all/id")]
        public async Task<IActionResult> FindFoldersByUserId([FromQuery(Name = "id")] Guid id)
        {
            return Ok(await m_folderService.GetFoldersByUserIdAsync(id));
        }






        /*
         * 
         * 
         * Rename folder with given folder id, new folder name parameters
         * 
         */
        [HttpPost("rename/dir")]
        public async Task<IActionResult> RenameFolder([FromQuery(Name = "id")] long folderId, [FromQuery(Name = "n")] string newFolderName)
        {
            m_folderService.RenameFolder(folderId, newFolderName);

            return Ok();
        }






        /*
         * 
         * 
         * Remove folder with given folder id parameter
         * 
         */
        [HttpDelete("remove/dir")]
        public async Task<IActionResult> DeleteDirectory([FromQuery(Name = "id")] long folderId)
        {
            return Ok(await m_folderService.DeleteFolder(folderId));

        }
    }
}