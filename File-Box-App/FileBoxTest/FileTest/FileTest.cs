using AutoMapper;
using Presentation.Controllers;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.FileServicePath;
using System.Globalization;

namespace FileBoxTest.FileTest
{
    public class FileTest
    {
        private readonly IFileService m_fileService;
        private readonly FileController m_fileController;

        private readonly FileSaveDto m_successFileSaveDto = new FileSaveDto
        {
            fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".can",
            fileType = ".can",
            userId = Util.USER_ID,
            folderId = 23
        };

        private readonly FileSaveDto m_invalidUserIdFileSaveDto = new FileSaveDto
        {
            fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".can",
            fileType = ".can",
            userId = "A884A7C2-B171-4C01-83CE-7C1CE25FD537", 
            folderId = 23
        };

        private readonly FileSaveDto m_invalidFolderIdFileSaveDto = new FileSaveDto
        {
            fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".can",
            fileType = ".can",
            userId = Util.USER_ID, //nuricanozturk
            folderId = 23
        };
        public FileTest()
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
            m_fileController = new FileController(m_fileService);
        }

        private async Task<FileViewDto> getLastCreatedFile()
        {
            var files = await m_fileService.SortFilesByCreationDateAsync(m_successFileSaveDto.folderId, Guid.Parse(m_successFileSaveDto.userId));

            return files.Last();
        }





        /*
         * 
         * Testing Create file with given parameter is fileSaveDto. Expected Success!
         * 
         */
        [Fact]
        public async void CreateFile_WithGivenFileSaveDto_ShouldReturnsTrue()
        {
            var createdFile = await m_fileService.CreateFile(m_successFileSaveDto);

            Assert.True(createdFile);
        }





        /*
         * 
         * Testing Create file with given parameter is fileSaveDto. Expected Fail!
         * 
         */
        [Fact]
        public async void CreateFile_WithGivenFileSaveDto_ShouldThrowsServiceException()
        {
            var expectedMessage = "You cannot access this folder!";
            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_fileService.CreateFile(m_invalidUserIdFileSaveDto));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }





        /*
         * 
         * Testing Remove file with given parameters are fileId and userId. Expected Success!
         * 
         */
        [Fact]
        public async void RemoveFile_WithGivenFileIdAndUserId_ShouldReturnsEqual()
        {
            var lastCreatedFile = await getLastCreatedFile();

            var removeFileOperation = await m_fileService.DeleteFile(lastCreatedFile.FileId, Guid.Parse(m_successFileSaveDto.userId));

            var expectedFileName = lastCreatedFile.FileName;

            Assert.Equal(expectedFileName, removeFileOperation);
        }





        /*
         * 
         * Testing Remove file with given parameters are fileId and userId. Expected Fail!
         * 
         */
        [Fact]
        public async void RemoveFile_WithGivenInvalidFileIdAndUserId_ShouldThrowsServiceException()
        {
            var lastCreatedFile = await getLastCreatedFile();

            var expectedMessage = "You cannot access this folder!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_fileService.DeleteFile(lastCreatedFile.FileId, Guid.Parse(m_invalidUserIdFileSaveDto.userId)));

            Assert.Equal(expectedMessage, exception.GetMessage);                        
        }





        /*
         * 
         * Testing Rename file with given parameters are fileId, newFileName and userId. Expected Success!
         * 
         */
        [Fact]
        public async void RenameFile_WithGivenFileIdNewFileNameAndUserId_ShouldReturnsEqual()
        {
            var lastCreatedFile = await getLastCreatedFile();
            var newFileName = Path.GetRandomFileName();
            var renamedFile = await m_fileService.RenameFile(lastCreatedFile.FileId, newFileName, Guid.Parse(m_successFileSaveDto.userId));

            Assert.NotNull(renamedFile);
            Assert.Equal(newFileName, renamedFile.FileName);
            Assert.NotEqual(lastCreatedFile.FilePath, renamedFile.FilePath);
            Assert.Equal(lastCreatedFile.FileSize, renamedFile.FileSize);
        }





        /*
         * 
         * Testing Rename file with given parameters are fileId, newFileName and userId. Expected Fail!
         * 
         */
        [Fact]
        public async void RenameFile_WithGivenInvalidFileIdNewFileNameAndUserId_ShouldThrowsServiceException()
        {            
            var lastCreatedFile = await getLastCreatedFile();
            var newFileName = Path.GetRandomFileName();            
            var expectedMessage = "You cannot access this folder!";

            var exception = await Assert.ThrowsAsync<ServiceException>(
                async () => await m_fileService.RenameFile(lastCreatedFile.FileId, newFileName, Guid.Parse(m_invalidUserIdFileSaveDto.userId)));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }







        /*
         * 
         * Testing Filter files by selected extension(.pdf) with given parameters are folderId, extension and UserId. Expected Success!
         * 
         */
        [Fact]
        public async void GetFilesByFileExtension_WithGivenFolderIdUserIdAndFileExtension_ShouldReturnsEqual()
        {
            var filesOnFolders = await m_fileService
                .GetFilesByFileExtensionAndFolderIdAsync
                    (m_successFileSaveDto.folderId, ".pdf",Guid.Parse(m_successFileSaveDto.userId));

            Assert.NotNull(filesOnFolders);

            var filtered = filesOnFolders.Select(f => f.FileType).Distinct();
            var actualFileCount = filtered.Count();
            var expectedFileCountAfterDistinct = 1;

            Assert.Equal(expectedFileCountAfterDistinct, actualFileCount);

            var expectedExtension = ".pdf";
            var actualExtension = filtered.First();

            Assert.Equal(expectedExtension, actualExtension);

        }





        /*
         * 
         * Testing Filter files by selected extension(.pdf) with given parameters are folderId, extension and UserId. Expected Fail!
         * 
         */
        [Fact]
        public async void GetFilesByFileExtension_WithGivenInvalidFolderIdAndFileExtension_ShouldThrowsServiceException()
        {
            var filesOnFolders = async () =>  await m_fileService.GetFilesByFileExtensionAndFolderIdAsync(m_successFileSaveDto.folderId, ".pdf",
              Guid.Parse(m_invalidUserIdFileSaveDto.userId));

            var expectedMessage = "You cannot access this folder!";
            var exception = await Assert.ThrowsAsync<ServiceException>(filesOnFolders);

            Assert.Equal(expectedMessage, exception.GetMessage);

        }





        /*
         * 
         * Testing sort files by file size (bigger to smaller) with given parameters are folderId and UserId. Expected Success!
         * 
         */
        [Fact]
        public async void SortFilesByFileBytes_WithGivenFolderIdAndUserId_ShouldReturnsTrue()
        {
            var sortedFiles = await m_fileService
                .SortFilesByFileBytesAsync(m_successFileSaveDto.folderId, Guid.Parse(m_successFileSaveDto.userId));

            Assert.NotNull(sortedFiles);

            var fileByteList = sortedFiles.Select(f => f.FileSize).ToList();

            var isSorted = true;

            for(int i = 0; i < fileByteList.Count - 1; i++)
                if (fileByteList[i] < fileByteList[i + 1])
                {
                    isSorted = false; 
                    break;
                }    

            Assert.True(isSorted);
        }





        /*
         * 
         * Testing sort files by creation date (old to new) with given parameters are folderId and UserId. Expected Success!
         * 
         */
        [Fact]
        public async void SortFilesByCreationDates_WithGivenFolderIdAndUserId_ShouldReturnsTrue()
        {
            var sortedFiles = await m_fileService.SortFilesByCreationDateAsync(m_successFileSaveDto.folderId, Guid.Parse(m_successFileSaveDto.userId));

            Assert.NotNull(sortedFiles);
            var format = "dd/MM/yyyy HH:mm:ss";
            var fileCreationDateList = sortedFiles.Select(f => DateTime.ParseExact(f.CreatedDate, format, CultureInfo.InvariantCulture, DateTimeStyles.None)).ToList();

            var isSorted = true;

            for (int i = 0; i < fileCreationDateList.Count - 1; i++)
                if (fileCreationDateList[i] > fileCreationDateList[i + 1])
                {
                    isSorted = false;
                    break;
                }

            Assert.True(isSorted);
        }        
    }
}