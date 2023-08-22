using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Exceptions;
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

                return Ok(new ResponseMessage(true, "file created successfully!", new FileResponseSuccessWithFileNameAndOwner(fileSaveDto.fileName, fileSaveDto.userId)));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * 
         * Remove file with given file id parameter
         * 
         */

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveFile([FromQuery(Name = "id")] long fileId, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var removedFileName = await m_fileService.DeleteFile(fileId, Guid.Parse(uid));

                return Ok(new ResponseMessage(true, "file removed successfully!", new FileDeleteSuccessResponse(removedFileName)));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }     
        }






        /*
         * 
         * 
         * Rename file with given file id and new file name
         * 
         */

        [HttpPost("rename")]
        public async Task<IActionResult> RenameFile([FromQuery(Name = "fid")] long fileId, [FromQuery(Name = "n")] string newFileName, [FromQuery(Name = "uid")] string userId)
        {
            try
            {
                var oldFile = await m_fileService.RenameFile(fileId, newFileName, Guid.Parse(userId));

                return Ok(new ResponseMessage(true, "Rename operation is successful!", new
                {
                    file = oldFile
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * Get all files from db with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        [HttpGet("find/all/folder")]
        public async Task<IActionResult> FindFilesByFolderIdAsync([FromQuery(Name = "fid")] long folderId, [FromQuery(Name = "uid")] string userId)
        {
            try
            {
                var currentToken = HttpContext.Request.Headers["Authorization"].ToString();                
                var filesOnFolders = await m_fileService.GetFilesByFolderIdAsync(folderId, Guid.Parse(userId), currentToken);
                return Ok(new ResponseMessage(true, $"Found {filesOnFolders.Count()} items.", new FileResponseFileList(filesOnFolders)));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }





        /*
         * 
         * Get all files from db with given folder ıd, user ıd and file extension parameter
         * 
         * 
         */
        [HttpGet("find/all/extension")]
        public async Task<IActionResult> FindFilesByFileExtensionAndFolderIdAsync([FromQuery(Name = "fid")] long folderId,
                                                                                  [FromQuery(Name = "ext")] string extension,
                                                                                  [FromQuery(Name = "uid")] string userId)
        {
            try
            {
                var filesOnFolders = await m_fileService.GetFilesByFileExtensionAndFolderIdAsync(folderId, extension, Guid.Parse(userId));
                return Ok(new ResponseMessage(true, $"Found {filesOnFolders.Count()} items.", new FileResponseFileList(filesOnFolders)));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






       /*
        * 
        * Sort files with given folder ıd, user ıd and file extension parameter
        * 
        * 
        */
        [HttpGet("sort/byte")]
        public async Task<IActionResult> SortFilesByFileBytesAsync([FromQuery(Name = "fid")] long folderId,
                                                                   [FromQuery(Name = "uid")] string userId)
        {
            try
            {
                var filesOnFolders = await m_fileService.SortFilesByFileBytesAsync(folderId, Guid.Parse(userId));
                return Ok(new ResponseMessage(true, $"Found {filesOnFolders.Count()} items.", new FileResponseFileList(filesOnFolders)));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }





      /*
       * 
       * Sort files with given folder ıd, user ıd and file extension parameter
       * 
       * 
       */
        [HttpGet("sort/date/creation")]
        public async Task<IActionResult> SortFilesByCreationDateAsync([FromQuery(Name = "fid")] long folderId,
                                                                   [FromQuery(Name = "uid")] string userId)
        {
            try
            {
                var filesOnFolders = await m_fileService.SortFilesByCreationDateAsync(folderId, Guid.Parse(userId));
                return Ok(new ResponseMessage(true, $"Found {filesOnFolders.Count()} items.", new FileResponseFileList(filesOnFolders)));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }
    }
}