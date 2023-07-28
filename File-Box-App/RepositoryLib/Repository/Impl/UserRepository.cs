using Microsoft.EntityFrameworkCore;
using RepositoryLib.Models;
using System.Linq.Expressions;

namespace RepositoryLib.Repository.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly FileBoxDbContext m_context;

        public UserRepository(FileBoxDbContext context) => m_context = context;


        /*
         * 
         * Get all users from db 
         * 
         */
        public IEnumerable<FileboxUser> All => m_context.FileboxUsers;







        /*
         * 
         * Get user count from db
         * 
         */
        public long Count()
        {
            return m_context.FileboxUsers.Count();
        }










        /*
         * 
         * Get user count async from db
         * 
         */
        public async Task<long> CountAsync()
        {
            return await m_context.FileboxUsers.CountAsync();
        }










        /*
         * 
         * Delete user with given parameter
         * 
         */
        public void Delete(FileboxUser t)
        {
            m_context.FileboxUsers.Remove(t);
        }










        /*
         * 
         * Delete User By Id 
         * 
         */
        public void DeleteById(Guid id)
        {
            var user = m_context.FileboxUsers.FirstOrDefault(usr => usr.UserId == id);
            m_context.FileboxUsers.Remove(user);
        }











        /*
         * 
         * Delete User By Id async
         * 
         */
        public async void DeleteByIdAsync(Guid id)
        {
            var user = m_context.FileboxUsers.FirstOrDefaultAsync(usr => usr.UserId == id).Result;
            await Task.Run(() => m_context.FileboxUsers.Remove(user));
        }








        /*
         * 
         * Exists User By Id 
         * 
         */
        public bool ExistsById(Guid id)
        {
            return m_context.FileboxUsers.FirstOrDefault(usr => usr.UserId == id) != null;
        }










        /*
         * 
         * Exists User By Id async
         * 
         */
        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            return await Task.Run(() => m_context.FileboxUsers.FirstOrDefaultAsync(usr => usr.UserId != id).Result) != null;
        }









        /*
         * 
         * Find All Users async
         * 
         */
        public async Task<IEnumerable<FileboxUser>> FindAllAsync()
        {
            return await Task.Run(() => m_context.FileboxUsers);
        }








        /*
         * 
         * Find Users by predicate
         * 
         */
        public IEnumerable<FileboxUser> FindByFilter(Expression<Func<FileboxUser, bool>> predicate)
        {
            return m_context.FileboxUsers.Where(predicate);
        }











        /*
         * 
         * Find Users by predicate async
         * 
         */
        public async Task<IEnumerable<FileboxUser>> FindByFilterAsync(Expression<Func<FileboxUser, bool>> predicate)
        {
            return await Task.Run(() => m_context.FileboxUsers.Where(predicate));
        }












        /*
         * 
         * Find User by id
         * 
         */
        public FileboxUser FindById(Guid id)
        {
            // if not found return null
            return m_context.FileboxUsers.FirstOrDefault(usr => usr.UserId == id);
        }


        /*
         * 
         * Find User by id async
         * 
         */
        public async Task<FileboxUser> FindByIdAsync(Guid id)
        {
            return await Task.Run(() => m_context.FileboxUsers.FirstOrDefaultAsync(usr => usr.UserId == id));
        }









        /*
         * 
         * Save User to database
         * 
         */
        public FileboxUser Save(FileboxUser t)
        {
            var user = m_context.FileboxUsers.Add(t).Entity;
            m_context.SaveChanges();
            return user;
        }











        /*
         * 
         * Save user to database async
         * 
         */
        public async Task<FileboxUser> SaveAsync(FileboxUser t)
        {
            var user = await Task.Run(() => m_context.FileboxUsers.AddAsync(t).Result.Entity);
            m_context.SaveChanges();
            return user;
        }


        //-------------------------------------------------------------------------------

        public void DeleteAsync(FileboxUser t)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<FileboxUser> Save(IEnumerable<FileboxUser> entities)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<FileboxUser> FindByIds(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxUser>> FindByIdsAsync(IEnumerable<Guid> ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<FileboxUser>> SaveAsync(IEnumerable<FileboxUser> entities)
        {
            throw new NotImplementedException();
        }
    }
}
