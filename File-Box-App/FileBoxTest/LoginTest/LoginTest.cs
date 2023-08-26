using FileBoxService.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Presentation.Controllers;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using Service.Exceptions;
using Service.Services.TokenService;

namespace FileBoxTest.LoginTest
{
    public class LoginTest
    {

        private readonly IUserLoginService m_loginService;
        private readonly LoginController m_loginController;


        private const string TEST_USER_ID = Util.USER_ID;

        private readonly UserLoginDTO m_successfullLoginDto = new UserLoginDTO
        {
            Username = "ahmetkoc",
            Password = "123"
        };


        private readonly UserLoginDTO m_unsuccessfullLoginDtoInvalidUsername = new UserLoginDTO
        {
            Username = "dogan",
            Password = "123"
        };


        private readonly UserLoginDTO m_unsuccessfullLoginDtoInvalidPassword = new UserLoginDTO
        {
            Username = "ahmetkoc",
            Password = "12345"
        };




        public LoginTest()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings_test.json").Build();

            var context = new FileBoxDbContext();
            var userRepoDal = new UserRepositoryDal(new CrudRepository<FileboxUser, Guid>(context));
            var folderRepoDal = new FolderRepositoryDal(new CrudRepository<FileboxFolder, long>(context));
            var tokenService = new TokenService(configuration);

            m_loginService = new LoginService(userRepoDal, folderRepoDal, tokenService);
            m_loginController = new LoginController(m_loginService);
        }





        /*
         * 
         * Given Correct user informations and returns user id equal to given user_id
         * 
         */
        [Fact]
        public async void LoginOperation_WithGivenLoginDto_ShouldReturnEqual()
        {            
            var result = await m_loginService.Login(m_successfullLoginDto);

            Assert.Equal(TEST_USER_ID.ToLower(), result.uid.ToLower());
            
            Assert.NotNull(result.token);
        }





        /*
         * 
         * Given Correct user informations and returns OkObjectstatus
         * 
         */
        [Fact]
        public async void LoginOperation_WithGivenLoginDto_ShouldReturnOkObject()
        {
            var result = await m_loginController.Login(m_successfullLoginDto);

            Assert.IsType<OkObjectResult>(result);
        }





        /*
         * 
         * Given invalid user informations and returns UnauthorizedObjectResult.
         * 
         */
        [Fact]
        public async void LoginOperation_GivenInvalidUsername_ShouldReturnUnauthorizedObjectResult()
        {
            var result = await m_loginController.Login(m_unsuccessfullLoginDtoInvalidUsername);
            Assert.IsType<UnauthorizedObjectResult>(result);
        }





        /*
         * 
         *  Given invalid user informations and should throw ServiceException. Also compare exception messages.
         * 
         */
        [Fact]
        public async void LoginOperation_GivenInvalidUsername_ShouldReturn401StatusCode()
        {            
            
            var expectedMessage = "Username or password is invalid!";
            
            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_loginService.Login(m_unsuccessfullLoginDtoInvalidUsername));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }




        /*
         * 
         *  Given invalid user informations and should throw ServiceException. Also compare exception messages.
         * 
         */
        [Fact]
        public async void LoginOperation_GivenInvalidPassword_ShouldReturn401StatusCode()
        {

            var expectedMessage = "Username or password is invalid!";

            var exception = await Assert.ThrowsAsync<ServiceException>(async () => await m_loginService.Login(m_unsuccessfullLoginDtoInvalidPassword));

            Assert.Equal(expectedMessage, exception.GetMessage);
        }
    }
}