﻿using AutoMapper;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.DownloadService;
using Service.Services.FileServicePath;
using Service.Services.FolderService;

namespace FileBoxTest.DownloadTest
{
    public class DownloadTest
    {
        private readonly IDownloadService m_downloadService;
        private readonly IFileService m_fileService;
        private readonly IFolderService m_folderService;

        private const long DOWNLOAD_SINGLE_FILE_ID = 304L;
        private const long DOWNLOAD_SINGLE_FOLDER_ID = 12L;
        private readonly Guid TESTED_USER_ID = Guid.Parse(Util.USER_ID);
        private readonly Guid TESTED_INVALID_USER_ID = Guid.Parse("D823C5B8-B149-4F1A-8650-74993089E430");
        public DownloadTest()
        {
            var context = new FileBoxDbContext();
            var folderRepoDal = new FolderRepositoryDal(new CrudRepository<FileboxFolder, long>(context));

            var fileRepoDal = new FileRepositoryDal(new CrudRepository<FileboxFile, long>(context));

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FolderViewDto, FileboxFolder>();
                cfg.CreateMap<FolderViewDto, FileboxFolder>().ReverseMap();
                cfg.CreateMap<FileboxFile, FileViewDto>().ReverseMap();
            }).CreateMapper();
            var unitOfWork = new UnitOfWork();
            m_fileService = new FileService(fileRepoDal, folderRepoDal, mapper, unitOfWork);
            m_downloadService = new DownloadService(fileRepoDal, folderRepoDal);
            m_folderService = new FolderService(folderRepoDal, mapper, fileRepoDal, new UnitOfWork());
        }





        /*
         * 
         * Testing Download File with given fileId and userId. expected success!
         * 
         */
      //  [Fact]
        public async void DownloadFile_WithGivenFileIdAndUserId_ShouldReturnEqual()
        {
            var downloadingFile = await m_fileService.FindFileByFileId(DOWNLOAD_SINGLE_FILE_ID, TESTED_USER_ID);
            Assert.NotNull(downloadingFile);
            var downloadFile = await m_downloadService.DownloadSingleFile(DOWNLOAD_SINGLE_FILE_ID, TESTED_USER_ID);

            Assert.Equal(downloadFile.Item3, downloadingFile.FileName);
        }





        /*
         * 
         * Testing Download File with given fileId and userId. expected fail!
         * 
         */
       // [Fact]
        public async void DownloadFile_WithGivenFileIdAndInvalidUserId_ShouldThrowServiceException()
        {
            var downloadingFile = await m_fileService.FindFileByFileId(DOWNLOAD_SINGLE_FILE_ID, TESTED_USER_ID);

            Assert.NotNull(downloadingFile);

            var expectedMessage = "You cannot access this folder!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_downloadService.DownloadSingleFile(DOWNLOAD_SINGLE_FILE_ID, TESTED_INVALID_USER_ID));
                        

            Assert.Equal(expectedMessage, exception.GetMessage);
        }
       
    }
}
