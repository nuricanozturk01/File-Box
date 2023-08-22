using FileBoxService.Service;
using Microsoft.Extensions.Configuration;
using Moq;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.TokenService;
using System.Linq.Expressions;

namespace FileBoxTest.MockTests
{
    public class LoginTestMock
    {
        private readonly Mock<IGenericRepository<FileboxFolder, long>> m_folderRepository;
        private readonly Mock<IGenericRepository<FileboxUser, Guid>> m_userRepository;
        private readonly TokenService m_tokenService;

        private const string TEST_USER_ID = Util.USER_ID;

        private readonly UserLoginDTO m_successfullLoginDto = new UserLoginDTO
        {
            Username = "ahmetkoc",
            Password = "123"
        };

        private readonly UserLoginDTO m_unsuccessfullLoginDtoInvalidPassword = new UserLoginDTO
        {
            Username = "ahmetkoc",
            Password = "12345"
        };


        public LoginTestMock()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings_test.json").Build();
            m_tokenService = new TokenService(configuration);

            m_folderRepository = new Mock<IGenericRepository<FileboxFolder, long>>();

            m_userRepository = new Mock<IGenericRepository<FileboxUser, Guid>>();
        }






        /*
         * 
         * Given Correct user informations and returns user id equal to given user_id
         * 
         */
        [Fact]
        public async void Mock_LoginOperation_WithGivenLoginDto_ShouldReturnEqual()
        {
            //Arrange
            m_folderRepository.Setup(service => service.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFolder, bool>>>())).ReturnsAsync(MockDataUtil.GetFileboxFolder);
            m_userRepository.Setup(service => service.FindByFilterAsync(It.IsAny<Expression<Func<FileboxUser, bool>>>())).ReturnsAsync(MockDataUtil.GetFileBoxUser);

            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);
            var userDal = new UserRepositoryDal(m_userRepository.Object);

            //Act
            var loginService = new LoginService(userDal, folderDal, m_tokenService);
            var result = await loginService.Login(m_successfullLoginDto);

            //Assert
            Assert.Equal(TEST_USER_ID.ToLower(), result.uid.ToLower());
            Assert.NotNull(result.token);
        }





        /*
         * 
         * Given invalid user informations and returns UnauthorizedObjectResult.
         * 
         */
        [Fact]
        public async void LoginOperation_GivenInvalidUsername_ShouldReturnUnauthorizedObjectResult()
        {
            //Arrange
            m_folderRepository.Setup(service => service.FindByFilterAsync(It.IsAny<Expression<Func<FileboxFolder, bool>>>())).ReturnsAsync(MockDataUtil.GetFileboxFolder);
            // user is null
            m_userRepository.Setup(service => service.FindByFilterAsync(It.IsAny<Expression<Func<FileboxUser, bool>>>())).ReturnsAsync(new List<FileboxUser>());

            var folderDal = new FolderRepositoryDal(m_folderRepository.Object);
            var userDal = new UserRepositoryDal(m_userRepository.Object);

            // Act
            var loginService = new LoginService(userDal, folderDal, m_tokenService);
            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await loginService.Login(m_unsuccessfullLoginDtoInvalidPassword));
            var expectedMessage = "Username or password is invalid!";


            Assert.Equal(expectedMessage, exception.GetMessage);
        }
    }
}