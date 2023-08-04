﻿using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;
using Service;
using Service.Exceptions;
using Service.Services.TokenService;


namespace FileBoxService.Service
{
    public class LoginService : IUserLoginService
    {
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly FolderRepositoryDal m_folderRepositoryDal;
        private readonly ITokenService m_tokenService;


        public LoginService(UserRepositoryDal userRepositoryDal, FolderRepositoryDal folderRepositoryDal, ITokenService tokenService)
        {
            m_userRepositoryDal = userRepositoryDal;
            m_folderRepositoryDal = folderRepositoryDal;
            m_tokenService = tokenService;
        }






        /*
         * 
         * 
         * Logout operation [NOT IMPLEMENTED YET]
         * 
         * 
         */
        public bool Logout(string username)
        {
            throw new NotImplementedException();
        }






        /*
         * 
         * 
         * If login operation is successfull, create the root file for user
         * 
         * 
         */
        internal async Task CreateDirectoryIfNotExists(string username, Guid userId, FileboxUser user)
        {
            var dirName = Util.DIRECTORY_BASE + username;

            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
                user.FileboxFolders.Add(new FileboxFolder(null, userId, username, username));
                m_userRepositoryDal.Update(user);
                m_userRepositoryDal.SaveChanges();
            }
            else
            {
                var rootFolder = (await m_folderRepositoryDal.FindByFilterAsync(folder => folder.UserId == user.UserId)).FirstOrDefault();

                if (rootFolder is null)
                {
                    await m_folderRepositoryDal.Save(new FileboxFolder(null, userId, username, username));
                    await m_folderRepositoryDal.SaveChangesAsync();
                }
            }
        }






        /*
         * 
         * 
         * Login operation for user with given userLoginDto parameter. 
         * returns the status of login operation
         * 
         */
        public async Task<string?> Login(UserLoginDTO userLoginDTO)
        {
            var user = (await m_userRepositoryDal.FindByFilterAsyncUser(user => user.Username == userLoginDTO.Username)).FirstOrDefault();

            if (user is null || !Util.VerifyPassword(userLoginDTO.Password, user.Password))
                throw new ServiceException("Username or password is invalid!");

            var token = m_tokenService.CreateToken(user.UserId.ToString());

            await CreateDirectoryIfNotExists(user.Username, user.UserId, user);
            user.LastToken = "Bearer " + token;
            await m_userRepositoryDal.SaveChangesAsync();

            return token;
        }
    }
}