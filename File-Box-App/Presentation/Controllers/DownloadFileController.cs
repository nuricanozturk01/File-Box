using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Exceptions;
using Service.Services.DownloadService;
using System.Net.Mime;

namespace Presentation.Controllers
{
    [Authorize]
    [Route("api/download")]
    [ApiController]
    public class DownloadFileController : ControllerBase
    {
        private readonly IDownloadService m_downloadService;

        public DownloadFileController(IDownloadService downloadService)
        {
            m_downloadService = downloadService;
        }






        /*
         * 
         * Download single with given filePath parameter 
         * 
         */
        [HttpGet("file")]
        public async Task<IActionResult> DownloadSingleFile([FromQuery(Name = "fid")] long fileId, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var (bytes, content, fileName) = await m_downloadService.DownloadSingleFile(fileId, Guid.Parse(uid));

                return File(bytes, content, fileName);

            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
           
        }






       /*
        * 
        * Firstly program zip the files then download zip with given filePath parameter 
        * 
        */
        [HttpGet("files")]
        public async Task<IActionResult> DownloadMultipleFile([FromBody] List<MultipleFileDownloadDto> filesDownloadDto, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var zipBytes = await m_downloadService.DownloadMultipleFile(filesDownloadDto, Guid.Parse(uid));


                var zipFileName = DateTime.Now.ToString("[yyyy-MM-dd HH.mm.ss]") + "_downloaded_files.zip";


                var contentDisposition = new ContentDisposition
                {
                    FileName = zipFileName,
                    Inline = false
                };

                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                return File(zipBytes, "application/zip", zipFileName);

            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
          
        }






        /*
         * 
         * Download folder with given filePath parameter 
         * 
         */
        [HttpGet("folder")]
        public async Task<IActionResult> DownloadSingleFolderZip([FromQuery(Name = "fid")] long folderId, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var (bytes, content, fileName) = await m_downloadService.DownloadSingleFolder(folderId, Guid.Parse(uid));

                return File(bytes, content, fileName);

            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * Download folder with given filePath parameter 
         * 
         */
        [HttpGet("folders")]
        public async Task<IActionResult> DownloadMultipleFolderZip([FromBody] List<MultipleFolderDownloadDto> folderDownloadDto, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var (bytes, content, fileName) = await m_downloadService.DownloadMultipleFolder(folderDownloadDto, Guid.Parse(uid));

                return File(bytes, content, fileName);

            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }
    }
}