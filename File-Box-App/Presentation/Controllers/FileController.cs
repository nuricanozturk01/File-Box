using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Services.FileServicePath;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/file")]
    public class FileController : ControllerBase
    {
        private readonly IFileService m_fileService;

        public FileController(IFileService fileService)
        {
            m_fileService = fileService;
        }






        /*
         * 
         * 
         * Create Empty File with given fileSaveDto parameter.
         * 
         */
        [HttpPost("create")]
        public async Task<IActionResult> CreateFile([FromBody] FileSaveDto fileSaveDto)
        {
            try
            {
                await m_fileService.CreateFile(fileSaveDto);

                return Ok("Created file " + fileSaveDto.fileName + " owner id is: " + fileSaveDto.userId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }






        /*
         * 
         * 
         * Remove file with given file id parameter
         * 
         */

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFile([FromQuery(Name = "id")] long fileId)
        {
            return Ok(await m_fileService.DeleteFile(fileId));
        }






        /*
         * 
         * 
         * Rename file with given file id and new file name
         * 
         */

        [HttpPut("rename")]
        public async Task<IActionResult> RenameFile([FromQuery(Name = "id")] long id, [FromQuery(Name = "n")] string newFileName)
        {
            m_fileService.RenameFile(id, newFileName);
            return Ok();
        }






        /*
         * 
         * 
         * Find folders specific folder and user with given user id and folder id.
         * 
         */
        [HttpGet("find/all/id")]
        public async Task<IActionResult> FindFoldersByUserId([FromQuery(Name = "uid")] string uid, [FromQuery(Name = "folderId")] long folderId)
        {
            return Ok(await m_fileService.GetFilesByUserIdAndFolderIdAsync(Guid.Parse(uid), folderId));
        }
    }
}
