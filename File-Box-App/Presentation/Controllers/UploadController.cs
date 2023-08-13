using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Exceptions;
using Service.Services.UploadService;

namespace Presentation.Controllers
{

    /*
     * 
     * This class Represent the Upload Single and Multiple Files and Folders 
     */

    [Authorize]
    [Route("api/upload")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService m_uploadService;


        public UploadController(IUploadService uploadService)
        {
            m_uploadService = uploadService;
        }




        /*
         * 
         * Upload single file.
         * 
         */
        [HttpPost("file")]
        public async Task<IActionResult> UploadSingleFile([FromQuery(Name = "uid")] string uid,
                                                          [FromQuery(Name = "fid")] long folderId,
                                                          [FromForm(Name = "formFile")] IFormFile formFile)
        {
            try
            {
                var uploadResult = await m_uploadService.UploadSingleFile(formFile, folderId, Guid.Parse(uid));
                
                return Ok(new ResponseMessage(true, "File Uploaded Successfully!", new UploadSingleFileResponse(uploadResult.path, uploadResult.totalLength + " MB")));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
           
        }







        /*
         * 
         * Upload multiple files.
         * 
         */
        [HttpPost("files")]
        public async Task<IActionResult> UploadMultipleFiles([FromQuery(Name = "uid")] string uid,
                                        [FromQuery(Name = "fid")] long folderId,
                                        [FromForm(Name = "formFile")] List<IFormFile> formFiles)
        {
            try
            {
                var uploadResult = await m_uploadService.UploadMultipleFiles(formFiles, folderId, Guid.Parse(uid));

                return Ok(new ResponseMessage(true, "File Uploaded Successfully!", new
                {
                    files = uploadResult
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }           
        }







        /*
         * 
         * Upload single folder.
         * 
         */
        [HttpPost("folder")]
        public async Task<IActionResult> UploadSingleFolder([FromBody] List<FolderUploadDto> sourceFolder,[FromQuery(Name = "uid")] string uid,[FromQuery(Name = "fid")] long folderId)
        {
            try
            {
                var uploadResult = await m_uploadService.UploadMultipleFolder(sourceFolder, folderId, Guid.Parse(uid));

                return Ok(new ResponseMessage(true, "File Uploaded Successfully!", new UploadFolderResponse(uploadResult.Select(up => up.sourceFilePath).ToList())));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * Upload multiple folders.
         * 
         */

        [HttpPost("folders")]
        public async Task<IActionResult> UploadMultipleFolders([FromBody] List<FolderUploadDto> sourceFolder, [FromQuery(Name = "uid")] string uid, [FromQuery(Name = "fid")] long folderId)
        {
            try
            {
                var uploadResult = await m_uploadService.UploadMultipleFolder(sourceFolder, folderId, Guid.Parse(uid));

                return Ok(new ResponseMessage(true, "Files Uploaded Successfully!", new UploadFolderResponse(uploadResult.Select(up => up.sourceFilePath).ToList())));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }
    }
}
