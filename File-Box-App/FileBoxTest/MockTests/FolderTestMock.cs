using AutoMapper;
using Moq;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.FolderService;
using System.Linq.Expressions;

namespace FileBoxTest.MockTests
{
    public class FolderTestMock
    {
        private readonly Mock<IGenericRepository<FileboxFolder, long>> m_folderRepository;
        private readonly Mock<IGenericRepository<FileboxFile, long>> m_fileRepositoy;
        private readonly IMapper m_mapper;
        public FolderTestMock() 
        {
            m_mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FolderViewDto, FileboxFolder>();
                cfg.CreateMap<FolderViewDto, FileboxFolder>().ReverseMap();
                cfg.CreateMap<FileboxFile, FileViewDto>().ReverseMap();
            }).CreateMapper();

            m_folderRepository = new Mock<IGenericRepository<FileboxFolder, long>>();

            m_fileRepositoy = new Mock<IGenericRepository<FileboxFile, long>>();
        }

        /*
         * 
         * Testing Find Root Folder with given parameter is user id. Expected Success!
         * 
         */
        [Fact]
        public async void FindRootFolder_WithGivenUserId_ShouldReturnEqual()
        {
            // Arrange
            m_folderRepository.Setup(repo => repo.FindAllAsync()).ReturnsAsync(MockDataUtil.GetFakeFoldersByUserCorrectUserId);
            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);

            // Act
            var folderService = new FolderService(folderDal, m_mapper, fileDal);

            var rootFolder = await folderService.FindRootFolder(Guid.Parse(Util.USER_ID));
            var expectedUserId = Util.USER_ID.ToLower();
            var actualUserId = rootFolder.userId.ToLower();

            //Assert
            Assert.Equal(expectedUserId, actualUserId);
        }





        /*
         * 
         * Testing Find Root Folder with given parameter is user id. Expected Fail!
         * 
         */
        [Fact]
        public async void FindRootFolder_WithGivenUserId_ShouldThrowServiceException()
        {
            // Arrange
            m_folderRepository.Setup(repo => repo.FindAllAsync()).ReturnsAsync(MockDataUtil.GetFakeFoldersByUserWrongUserId);
            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);

            // Act
            var folderService = new FolderService(folderDal, m_mapper, fileDal);
            var expectedMessage = "Folder is not found!";
            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await folderService.FindRootFolder(Guid.NewGuid()));
            
            //Assert
            Assert.IsType<ServiceException>(exception);
            Assert.Equal(expectedMessage, exception.GetMessage);
        }





        /*
         * 
         * Testing Find Folder with files with given parameters are user id. Expected Success!
         * 
         */
        [Fact]
        public async void FindFolderWithFiles_WithGivenUserId_ShouldReturnEqual()
        {
            // Arrange
            m_folderRepository
                .Setup(repo => repo.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFolder, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFoldersByUserCorrectUserId);

            m_fileRepositoy.Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles);

            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);

            // Act
            var folderService = new FolderService(folderDal, m_mapper, fileDal);

            var userIds = (await folderService.FindFolderWithFiles(Guid.Parse(Util.USER_ID))).Select(folder => folder.userId);

            var expectedUserId = Util.USER_ID.ToLower();

            //Assert
            Assert.Single(userIds.Distinct());

            var actualUserId = userIds.Distinct().First();

            Assert.Equal(expectedUserId, actualUserId);
        }






        /*
         * 
         * Testing Find Folder with given parameters are InvalidUserId and folderId. Expected Success!
         * 
         */
        [Fact]
        public async void FindFolderWithFolderId_WithGivenUserIdAndFolderId_ShouldReturnEqual()
        {
            // Arrange
            m_folderRepository
                .Setup(repo => repo.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFolder, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFoldersByUserCorrectUserId().Reverse);

            m_fileRepositoy.Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles);



            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);

            // Act
            var folderService = new FolderService(folderDal, m_mapper, fileDal);


            var userId = Guid.Parse(Util.USER_ID);

            var userFolders = await folderService.FindFolderWithFiles(userId);

            var lastCreatedFolder = userFolders.OrderByDescending(folder => folder.folderId).FirstOrDefault();

            var folder = await folderService.FindFolderWithFolderId(userId, lastCreatedFolder.folderId);

            //Assert
            Assert.Equal(lastCreatedFolder.folderName, folder.folderName);
            Assert.Equal(lastCreatedFolder.folderId, folder.folderId);
            Assert.Equal(lastCreatedFolder.folderPath, folder.folderPath);
            Assert.Equal(lastCreatedFolder.folderPath, folder.folderPath);
        }





        /*
         * 
         * Testing Find Folder with given parameters are InvalidUserId and folderId. Expected Fail!
         * 
         */
        [Fact]
        public async void FindFolderWithFolderId_WithGivenInvalidUserIdAndFolderId_ShouldThrowServiceException()
        {
            // Arrange
            m_folderRepository
                .Setup(repo => repo.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFolder, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFoldersByUserCorrectUserId().Reverse);

            m_fileRepositoy.Setup(r => r.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFile, bool>>>()))
                .ReturnsAsync(MockDataUtil.GetFakeFilteredFiles);



            var fileDal = new FileRepositoryDal(m_fileRepositoy.Object);
            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);

            // Act

            var folderService = new FolderService(folderDal, m_mapper, fileDal);
            var userId = Guid.NewGuid();

            var userFolders = await folderService.FindFolderWithFiles(Guid.Parse(Util.USER_ID));

            var lastCreatedFolder = userFolders.OrderByDescending(folder => folder.folderId).FirstOrDefault();

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await folderService.FindFolderWithFolderId(userId, lastCreatedFolder.folderId));

            var expectedMessage = "You cannot access this folder!";

            Assert.Equal(expectedMessage, exception.GetMessage);
        }

    }
}
