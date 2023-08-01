using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using RepositoryLib.DTO;
using System.IO.Compression;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace Service.Services.DownloadService
{
    public interface IDownloadService
    {
        Task<(byte[], string, string)> DownloadSingleFile(string filePath);
        Task<(byte[], string, string)> DownloadMultipleFile(List<FolderUploadDto> filePaths);
        Task<(byte[] bytes, string content, string fileName)> DownloadSingleFolder(string filePath);
        Task<(byte[] bytes, string content, string fileName)> DownloadMultipleFolder(List<FolderUploadDto> filePath);
    }
}
