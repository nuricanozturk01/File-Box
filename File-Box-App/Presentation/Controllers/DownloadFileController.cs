using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.DTO;
using Service.Services.DownloadService;

namespace Presentation.Controllers
{
   // [Authorize]
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
        public async Task<IActionResult> DownloadSingleFile([FromQuery(Name = "fpath")] string filePath)
        {
            var (bytes, content, fileName) = await m_downloadService.DownloadSingleFile(filePath);

            return File(bytes, content, fileName);
        }
        /*
         * 
         * Download folder with given filePath parameter 
         * 
         */
        [HttpGet("folder")]
        public async Task<IActionResult> DownloadSingleFolderZip([FromQuery(Name = "fpath")] string filePath)
        {
            var (bytes, content, fileName) = await m_downloadService.DownloadSingleFolder(filePath);

            return File(bytes, content, fileName);
        }

        /*
         * 
         * Download folder with given filePath parameter 
         * 
         */
        [HttpGet("folders")]
        public async Task<IActionResult> DownloadMultipleFolderZip([FromBody] List<FolderUploadDto> filePath)
        {
            var (bytes, content, fileName) = await m_downloadService.DownloadMultipleFolder(filePath);

            return File(bytes, content, fileName);
        }
        /*
         * 
         * Firsly program zip the files then download zip with given filePath parameter 
         * 
         */
        [HttpGet("files")]
        public async Task<IActionResult> DownloadMultipleFile([FromBody] List<FolderUploadDto> filePath)
        {
            var (bytes, content, fileName) = await m_downloadService.DownloadMultipleFile(filePath);

            return File(bytes, content, fileName);
        }
    }
}
