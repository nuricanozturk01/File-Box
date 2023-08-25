using AutoMapper;
using Moq;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.FileServicePath;
using System.Globalization;
using System.Linq.Expressions;

namespace FileBoxTest.MockTests
{
    public class FileTestMock
    {
        private readonly Mock<IGenericRepository<FileboxUser, Guid>> m_userRepository;
        private readonly Mock<IGenericRepository<FileboxFolder, long>> m_folderRepository;
        private readonly Mock<IGenericRepository<FileboxFile, long>> m_fileRepositoy;
        private readonly IMapper m_mapper;
        private const string TEST_USER_ID = Util.USER_ID;
        private const long TEST_FOLDER_ID = 24L;

        private readonly FileSaveDto m_successFileSaveDto = new FileSaveDto
        {
            fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".can",
            fileType = ".can",
            userId = Util.USER_ID,
            folderId = 24
        };

        private readonly FileSaveDto m_invalidUserIdFileSaveDto = new FileSaveDto
        {
            fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".can",
            fileType = ".can",
            userId = "A884A7C2-B171-4C01-83CE-7C1CE25FD537",
            folderId = 23
        };
        public FileTestMock()
        {
            m_mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FolderViewDto, FileboxFolder>();
                cfg.CreateMap<FolderViewDto, FileboxFolder>().ReverseMap();
                cfg.CreateMap<FileboxFile, FileViewDto>().ReverseMap();
            }).CreateMapper();

            m_folderRepository = new Mock<IGenericRepository<FileboxFolder, long>>();

            m_userRepository = new Mock<IGenericRepository<FileboxUser, Guid>>();

            m_fileRepositoy = new Mock<IGenericRepository<FileboxFile, long>>();
        }

        /*
         * 
         * Testing Create file with given parameter is fileSaveDto. Expected Success!
         * 
         */
        [Fact]
        public async void Mock_GetFilesByFileExtension_WithGivenFolderIdUserIdAndFileExtension_ShouldReturnsEqual()
        {
            // Arrange

            m_folderRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<long>())).ReturnsAsync(MockDataUtil.GetFakeFolder);

            m_fileRepositoy
                .Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles().Where(f => f.FileType == ".pdf"));

            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);
            var unitOfWork = new UnitOfWork();
            // Act
            var fileService = new FileService(fileDal, folderDal, m_mapper, unitOfWork);
            var filesOnFolders = await fileService
                .GetFilesByFileExtensionAndFolderIdAsync(TEST_FOLDER_ID, ".pdf", Guid.Parse(TEST_USER_ID));

            //Assert            
            Assert.NotNull(filesOnFolders);

            foreach (var item in filesOnFolders)
                Assert.Equal(".pdf", item.FileType);
        }





        /*
         * 
         * Testing Filter files by selected extension(.pdf) with given parameters are folderId, extension and UserId. Expected Fail!
         * 
         */
        [Fact]
        public async void GetFilesByFileExtension_WithGivenInvalidFolderIdAndFileExtension_ShouldThrowsServiceException()
        {
            // Arrange

            m_folderRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<long>())).ReturnsAsync(MockDataUtil.GetInvalidFakeFolder);

            m_fileRepositoy
                .Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles().Where(f => f.FileType == ".pdf"));

            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);
            var unitOfWork = new UnitOfWork();
            // Act
            var fileService = new FileService(fileDal, folderDal, m_mapper, unitOfWork);
            var filesOnFolders = async () => await fileService
                .GetFilesByFileExtensionAndFolderIdAsync(TEST_FOLDER_ID, ".pdf", Guid.Parse(Util.INVALID_USER_ID));

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
            // Arrange

            m_folderRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<long>())).ReturnsAsync(MockDataUtil.GetInvalidFakeFolder);

            m_fileRepositoy
                .Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles().OrderByDescending(file => file.FileSize));

            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);
            var unitOfWork = new UnitOfWork();
            // Act
            var fileService = new FileService(fileDal, folderDal, m_mapper, unitOfWork);
            var sortedFiles = await fileService.SortFilesByFileBytesAsync(TEST_FOLDER_ID, Guid.Parse(Util.USER_ID));

            Assert.NotNull(sortedFiles);

            var fileByteList = sortedFiles.Select(f => f.FileSize).ToList();

            var isSorted = true;

            for (int i = 0; i < fileByteList.Count - 1; i++)
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
            // Arrange

            m_folderRepository.Setup(repo => repo.FindByIdAsync(It.IsAny<long>())).ReturnsAsync(MockDataUtil.GetInvalidFakeFolder);

            m_fileRepositoy
                .Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles().OrderByDescending(file => file.CreatedDate));

            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);

            var unitOfWork = new UnitOfWork();
            // Act
            var fileService = new FileService(fileDal, folderDal, m_mapper, unitOfWork);
            var sortedFiles = await fileService.SortFilesByCreationDateAsync(TEST_FOLDER_ID, Guid.Parse(Util.USER_ID));

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
