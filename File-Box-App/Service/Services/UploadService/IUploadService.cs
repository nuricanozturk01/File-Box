using Microsoft.AspNetCore.Http;
using RepositoryLib.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services.UploadService
{
    public interface IUploadService
    {
        Task<bool> UploadSingleFile(IFormFile formFile, long folderId, Guid guid);
        Task<bool> UploadMultipleFiles(List<IFormFile> formFile, long folderId, Guid uid);
        Task<bool> UploadSingleFolder(FolderUploadDto sourcePath, long folderId, Guid uid);
        Task<bool> UploadMultipleFolder(List<FolderUploadDto> sourcePaths, long folderId, Guid uid);
    }
}
