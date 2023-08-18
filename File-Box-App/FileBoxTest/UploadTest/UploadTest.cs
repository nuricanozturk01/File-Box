using AutoMapper;
using Microsoft.AspNetCore.Http;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Services.FileServicePath;
using Service.Services.UploadService;

namespace FileBoxTest.UploadTest
{
    public class UploadTest
    {
        private IUploadService m_uploadService;
        private IFileService m_fileService;

        private const long TESTED_FOLDER_ID = 10L; // owner is nuricanozturk
        private readonly Guid TESTED_USER_ID = Guid.Parse("D285C5B8-B149-4F1A-8650-74993089E430");

        public UploadTest()
        {
            var context = new FileBoxDbContext();
            var userRepoDal = new UserRepositoryDal(new CrudRepository<FileboxUser, Guid>(context));
            var folderRepoDal = new FolderRepositoryDal(new CrudRepository<FileboxFolder, long>(context));

            var fileRepoDal = new FileRepositoryDal(new CrudRepository<FileboxFile, long>(context));

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FolderViewDto, FileboxFolder>();
                cfg.CreateMap<FolderViewDto, FileboxFolder>().ReverseMap();
                cfg.CreateMap<FileboxFile, FileViewDto>().ReverseMap();
            }).CreateMapper();
            m_uploadService = new UploadService(folderRepoDal, fileRepoDal, mapper);
            m_fileService = new FileService(fileRepoDal, folderRepoDal, mapper, userRepoDal);
        }


        // Create the text files for test
        private IFormFile GenerateFormFile(string fileName, string content)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            var formFile = new FormFile(stream, 0, stream.Length, "name", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            return formFile;
        }

        private List<IFormFile> CreateFileListSingle()
        {
            return new List<IFormFile>
            {
                GenerateFormFile(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt", "Test Single")
            };
        }


        private List<IFormFile> CreateFileListMultiple()
        {
            return new List<IFormFile>
            {
                GenerateFormFile(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt", "Test1"),
                GenerateFormFile(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt", "Test2"),
                GenerateFormFile(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt", "Test3"),
                GenerateFormFile(Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".txt", "Test4")
            };
        }






        /*
         * 
         * Upload Single File and Validate it 
         * 
         */
        [Fact]
        public async void UploadSingleFile_WithGivenFormFilesFolderIdAndUserId_ShouldReturnEqual()
        {
            var uploadedFileList = await m_uploadService.UploadMultipleFiles(CreateFileListSingle(),TESTED_FOLDER_ID,TESTED_USER_ID);

            Assert.NotNull(uploadedFileList);

            var fileId = uploadedFileList[0].FileId;
            var sortedFiles = await m_fileService.SortFilesByFileBytesAsync(TESTED_FOLDER_ID, TESTED_USER_ID);


            Assert.Contains(fileId, sortedFiles.Select(sf => sf.FileId));
        }






        /*
         * 
         * Upload multiple File and Validate it 
         * 
         */
        [Fact]
        public async void UploadMultipleFile_WithGivenFormFilesFolderIdAndUserId_ShouldReturnEqual()
        {


            var uploadedFileList = await m_uploadService.UploadMultipleFiles(CreateFileListMultiple(),TESTED_FOLDER_ID,TESTED_USER_ID);

            Assert.NotNull(uploadedFileList);

            var fileIds = uploadedFileList.Select(f => f.FileId);

            var sortedFiles = await m_fileService.SortFilesByFileBytesAsync(TESTED_FOLDER_ID, TESTED_USER_ID);

            var fileIdsOnDb = sortedFiles.Select(f => f.FileId);

            bool flag = true;

            foreach (var fid in fileIds)
            {
                if (!fileIdsOnDb.Contains(fid))
                {
                    flag = false;
                    break;
                }
            }
            Assert.True(flag);
        }
    }
}
