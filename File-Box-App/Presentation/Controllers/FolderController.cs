using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using Service.Exceptions;
using Service.Services.FolderService;

namespace Presentation.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/folder")]
    public class FolderController : ControllerBase
    {
        private readonly IFolderService m_folderService;
        private readonly UserRepositoryDal m_userRepositoryDal;
        public FolderController(IFolderService folderService, UserRepositoryDal userRepositoryDal)
        {
            m_folderService = folderService;
            m_userRepositoryDal = userRepositoryDal;
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

                return Ok(new ResponseMessage(true, "folder created succesfully!", new FolderCreatedResponseDto(folderSaveDTO.currentFolderPath, folderSaveDTO.newFolderName)));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseMessage(false, ex.Message, null));
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
            try
            {
                var folders = await m_folderService.GetFoldersByUserIdAsync(id);

                return Ok(new ResponseMessage(true, $"{folders.Count()} items found!", new
                { // I use anonymous class.

                    user_folders = folders
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * 
         * Find Root folder with given user id
         * 
         */
        [HttpGet("find/root/uuid")]
        public async Task<IActionResult> FindRootFolderByUserId([FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var folder = await m_folderService.FindRootFolder(Guid.Parse(uid));
                
                return Ok(new ResponseMessage(true, "Root folder is found!", new
                { // I use anonymous class.
                    folder_name = folder.folderName,
                    folder_path = folder.folderPath,
                    folder_id = folder.folderId
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * 
         * Rename folder with given folder id, new folder name parameters
         * 
         */
        [HttpPost("rename/dir")]
        public async Task<IActionResult> RenameFolder([FromQuery(Name = "id")] long folderId, [FromQuery(Name = "n")] string newFolderName, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var renameFolder = await m_folderService.RenameFolder(folderId, newFolderName, Guid.Parse(uid));

                return Ok(new ResponseMessage(true, "Folder rename operation is successful!", new
                {
                    folder_old_path = renameFolder.oldPath,
                    folder_new_path = renameFolder.newPath
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
            

            return Ok();
        }






        /*
         * 
         * 
         * Remove folder with given folder id parameter
         * 
         */
        [HttpDelete("remove/dir")]
        public async Task<IActionResult> DeleteDirectory([FromQuery(Name = "id")] long folderId, [FromQuery(Name = "uid")] string uid)
        {
            try
            {
                var removedFolder = await m_folderService.DeleteFolder(folderId, Guid.Parse(uid));

                return Ok(new ResponseMessage(true, "Folder removed successfully!", new
                {
                    removed_folder_name = removedFolder
                }));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }






        /*
         * 
         * 
         * Find Folders given user id
         * 
         */
        [HttpGet("find/all/folderfiles")]
        public async Task<IActionResult> FindFoldersWithFiles([FromQuery(Name = "id")] Guid id)
        {
            try
            {
                

                var token = HttpContext.Request.Headers["Authorization"].ToString();

                var user = await m_userRepositoryDal.FindByIdAsyncUser(id);


                if (token != user.LastToken)
                    throw new ServiceException("You cannot access these files!");

                var folders = await m_folderService.FindFolderWithFiles(id);
                return Ok(new ResponseMessage(true, $"{folders.Count()} folder found!", folders));
            }
            catch (ServiceException ex)
            {
                return StatusCode(500, new ResponseMessage(false, ex.GetMessage, null));
            }
        }
    }
}