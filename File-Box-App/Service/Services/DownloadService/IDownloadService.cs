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
        Task<(byte[], string, string)> DownloadSingleFile(long fileId, Guid uid);
        Task<byte[]> DownloadMultipleFile(List<MultipleFileDownloadDto> filesDownloadDtoi, Guid uid);
        Task<(byte[] bytes, string content, string fileName)> DownloadSingleFolder(long folderId, Guid uid);
        Task<(byte[] bytes, string content, string fileName)> DownloadMultipleFolder(List<MultipleFolderDownloadDto> folderDownloadDto, Guid uid);
    }
}
