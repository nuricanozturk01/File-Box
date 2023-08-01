using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
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
            var uploadResult = await m_uploadService.UploadSingleFile(formFile, folderId, Guid.Parse(uid));

            return uploadResult ? Ok("Uploaded succsesfully!") : BadRequest("Internal Server Error");
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
            var uploadResult = await m_uploadService.UploadMultipleFiles(formFiles, folderId, Guid.Parse(uid));

            return uploadResult ? Ok("Uploaded succsesfully!") : BadRequest("Internal Server Error");
        }







        /*
         * 
         * Upload single folder.
         * 
         */
        [HttpPost("folder")]
        public async Task<IActionResult> UploadSingleFolder(
                                        [FromBody] List<FolderUploadDto> sourceFolder,
                                        [FromQuery(Name = "uid")] string uid,
                                        [FromQuery(Name = "fid")] long folderId)
        {
            var uploadResult = await m_uploadService.UploadMultipleFolder(sourceFolder, folderId, Guid.Parse(uid));


            return uploadResult ? Ok("Uploaded succsesfully!") : BadRequest("Internal Server Error");
        }






        /*
         * 
         * Upload multiple folders.
         * 
         */

        [HttpPost("folders")]
        public async Task<IActionResult> UploadMultipleFolders([FromBody] List<FolderUploadDto> sourceFolder,
                                        [FromQuery(Name = "uid")] string uid,
                                        [FromQuery(Name = "fid")] long folderId)
        {
            var uploadResult = await m_uploadService.UploadMultipleFolder(sourceFolder, folderId, Guid.Parse(uid));

            return uploadResult ? Ok("Uploaded succsesfully!") : BadRequest("Internal Server Error");
        }
    }
}
