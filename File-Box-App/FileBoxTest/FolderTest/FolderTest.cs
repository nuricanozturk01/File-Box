using AutoMapper;
using Presentation.Controllers;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.FolderService;


namespace FileBoxTest.FolderTest
{
    public class FolderTest
    {
        private readonly IFolderService m_folderService;
        private readonly FolderController m_folderController;

        
        private FolderSaveDTO m_successCreateFolderDto = new FolderSaveDTO
        {
            userId = Util.USER_ID,
            currentFolderId = 23, // parent folder user of ahmetkoc
            newFolderName = Guid.NewGuid().ToString()
        };



        private FolderSaveDTO m_failCreateFolderDtoInvalidUserId = new FolderSaveDTO
        {
            userId = Guid.NewGuid().ToString(),
            currentFolderId = 23, // parent folder user of nuricanozturk
            newFolderName = Guid.NewGuid().ToString()
        };


        // other user's folder
        private FolderSaveDTO m_failCreateFolderDtoInvalidFolderId = new FolderSaveDTO
        {
            userId = Util.USER_ID, //user id is ahmet
            currentFolderId = 1, // parent folder user of nuricanozturk
            newFolderName = Guid.NewGuid().ToString()
        };

        public FolderTest()
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

            m_folderService = new FolderService(folderRepoDal, mapper, fileRepoDal, new UnitOfWork());

            m_folderController = new FolderController(m_folderService, userRepoDal);
        }






        /*
         * 
         * Testing Create Folder with given parameters are FolderSaveDto. Expected Success!
         * 
         */
        [Fact]
        public async void CreateFolder_WithGivenFolderSaveDto_ShoulReturnEqual()
        {
            var createFolderOperation = await m_folderService.CreateFolder(m_successCreateFolderDto);

            var userFolders = await m_folderService.FindFolderWithFolderId(Guid.Parse(m_successCreateFolderDto.userId), createFolderOperation.folderId);

            Assert.Equal(createFolderOperation.folderId, userFolders.folderId);
        }





        /*
         * 
         * Testing Create Folder with given parameters are FolderSaveDto. but invalid user id. Expected Fail!
         * 
         */
        [Fact]
        public async void CreateFolder_WithGivenFolderSaveDtoWithInvalidUserId_ShouldThrowServiceException()
        {

            var expectedMessage = "Folder owner not this user!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_folderService.CreateFolder(m_failCreateFolderDtoInvalidUserId));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }





        /*
         * 
         * Testing Create Folder with given parameters are FolderSaveDto. Expected Fail!
         * 
         */
        [Fact]
        public async void CreateFolder_WithGivenFolderSaveDtoWithInvalidFolderId_ShouldThrowServiceException()
        {
            var expectedMessage = "Folder owner not this user!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_folderService.CreateFolder(m_failCreateFolderDtoInvalidFolderId));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }





        /*
         * 
         * Testing Remove Folder with given parameters are folderId and userId. Expected Success!
         * 
         */
        [Fact]
        public async void RemoveFolder_WithGivenFolderIdAndUserId_ShouldReturnEqual()
        {
            var userFolders = await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId));

            var lastCreatedFile = userFolders.OrderByDescending(folder => folder.folderId);

            var removedFile = lastCreatedFile.FirstOrDefault();

            var removeFolderOperation = await m_folderService.DeleteFolder(removedFile.folderId, Guid.Parse(m_successCreateFolderDto.userId));

            Assert.Equal(removedFile.folderPath, removeFolderOperation);
        }





        /*
         * 
         * Testing Remove Folder with given parameters are folderId and userId. Expected Fail!
         * 
         */
        [Fact]
        public async void RemoveFolder_WithGivenInvalidUserId_ShouldThrowServiceException()
        {
            var userFolders = await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId));

            var lastCreatedFile = userFolders.OrderByDescending(folder => folder.folderId);

            var removedFile = lastCreatedFile.FirstOrDefault();

            var expectedMessage = "You cannot access this folder!";

            var exception = await Assert.ThrowsAsync<ServiceException>
                (async () => await m_folderService.DeleteFolder(removedFile.folderId, Guid.Parse(m_failCreateFolderDtoInvalidUserId.userId)));


            Assert.Equal(expectedMessage, exception.GetMessage);
        }





        /*
         * 
         * Testing Remove Folder with given parameters are folderId and userId. Expected Fail!
         * 
         */
        [Fact]
        public async void RemoveFolder_WithGivenInvalidFolderId_ShouldThrowServiceException()
        {
            var userFolders = await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId));

            var lastCreatedFile = userFolders.OrderByDescending(folder => folder.folderId);

            var removedFile = lastCreatedFile.FirstOrDefault();

            var expectedMessage = "You cannot access this folder!";

            var exception = await Assert.ThrowsAsync<ServiceException>
                (async () => await m_folderService.DeleteFolder(m_failCreateFolderDtoInvalidFolderId.currentFolderId, Guid.Parse(m_failCreateFolderDtoInvalidUserId.userId)));


            Assert.Equal(expectedMessage, exception.GetMessage);
        }






        /*
         * 
         * Testing Rename Folder with given parameters are folderId, usreId. Expected Success!
         * 
         */
        [Fact]
        public async void RenameFolder_WithGivenFolderIdNewFolderNameAndUserId_ShouldReturnNotEqual()
        {
            var userFolders = await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId));

            var lastCreatedFolder = userFolders.OrderByDescending(folder => folder.folderId).FirstOrDefault();

            var result = await m_folderService.RenameFolder(lastCreatedFolder.folderId, Guid.NewGuid().ToString(), Guid.Parse(m_successCreateFolderDto.userId));

            Assert.Equal(result.folderId, lastCreatedFolder.folderId);
            Assert.Equal(result.creationDate, lastCreatedFolder.creationDate);
            Assert.NotEqual(result.folderName, lastCreatedFolder.folderName);
            Assert.NotEqual(result.folderPath, lastCreatedFolder.folderPath);
        }





        /*
         * 
         * Testing Rename Folder with given parameters are folderId, usreId. Expected Fail!
         * 
         */
        [Fact]
        public async void RenameFolder_WithGivenFolderIdNewFolderNameAndInvalidUserId_ShouldThrowServiceException()
        {
            var userFolders = await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId));

            var lastCreatedFolder = userFolders.OrderByDescending(folder => folder.folderId).FirstOrDefault();

            var expectedMessage = "You cannot access this folder!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () =>
            await m_folderService.RenameFolder(lastCreatedFolder.folderId, Guid.NewGuid().ToString(), Guid.Parse(m_failCreateFolderDtoInvalidUserId.userId)));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }





        /*
         * 
         * Testing Find Root Folder with given parameter is user id. Expected Success!
         * 
         */
        [Fact]
        public async void FindRootFolder_WithGivenUserId_ShouldReturnEqual()
        {
            var rootFolder = await m_folderService.FindRootFolder(Guid.Parse(m_successCreateFolderDto.userId));
            var expectedUserId = m_successCreateFolderDto.userId.ToLower();
            var actualUserId = rootFolder.userId.ToLower();

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
            var expectedMessage = "Folder is not found!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_folderService.FindRootFolder(Guid.NewGuid()));

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
            var userIds = (await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId))).Select(folder => folder.userId);

            var expectedUserId = m_successCreateFolderDto.userId.ToLower();

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
            var userId = Guid.Parse(m_successCreateFolderDto.userId);

            var userFolders = await m_folderService.FindFolderWithFiles(userId);

            var lastCreatedFolder = userFolders.OrderByDescending(folder => folder.folderId).FirstOrDefault();

            var folder = await m_folderService.FindFolderWithFolderId(userId, lastCreatedFolder.folderId);

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
            var userId = Guid.Parse(m_failCreateFolderDtoInvalidUserId.userId);

            var userFolders = await m_folderService.FindFolderWithFiles(Guid.Parse(m_successCreateFolderDto.userId));

            var lastCreatedFolder = userFolders.OrderByDescending(folder => folder.folderId).FirstOrDefault();

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_folderService.FindFolderWithFolderId(userId, lastCreatedFolder.folderId));

            var expectedMessage = "You cannot access this folder!";

            Assert.Equal(expectedMessage, exception.GetMessage);
        }
    }
}
