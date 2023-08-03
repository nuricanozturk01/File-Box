using RepositoryLib.Models;
using RepositoryLib.Repository;
using System.Linq.Expressions;

namespace RepositoryLib.Dal
{
   /*
    * 
    * Helper class for user repository. This class is a facade pattern for repositories 
    * 
    */
    public class UserRepositoryDal
    {
        private readonly IUserRepository m_userRepository;

        public UserRepositoryDal(IUserRepository userRepository)
        {
            m_userRepository = userRepository;
        }

        public long CountUser()
        {
            return m_userRepository.Count();
        }

        public async Task<long> CountAsyncUser()
        {
            return await m_userRepository.CountAsync();
        }

        public void DeleteUser(FileboxUser t)
        {
            m_userRepository.Delete(t);
        }

        public void DeleteByIdUser(Guid id)
        {
            m_userRepository.DeleteById(id);
        }

        public async void DeleteByIdAsyncUser(Guid id)
        {
            await Task.Run(() => m_userRepository.DeleteByIdAsync(id));
        }

        public bool ExistsByIdUser(Guid id)
        {
            return m_userRepository.ExistsById(id);
        }

        public async Task<bool> ExistsByIdAsyncUser(Guid id)
        {
            return await Task.Run(() => m_userRepository.ExistsByIdAsync(id));
        }

        public async Task<IEnumerable<FileboxUser>> FindAllAsyncUser()
        {
            return await Task.Run(() => m_userRepository.FindAllAsync());
        }
        public IEnumerable<FileboxUser> FindAllUser()
        {
            return m_userRepository.All;
        }
        public IEnumerable<FileboxUser> FindByFilterUser(Expression<Func<FileboxUser, bool>> predicate)
        {
            return m_userRepository.FindByFilter(predicate);
        }

        public async Task<IEnumerable<FileboxUser>> FindByFilterAsyncUser(Expression<Func<FileboxUser, bool>> predicate)
        {
            return await Task.Run(() => m_userRepository.FindByFilterAsync(predicate));
        }

        public FileboxUser FindByIdUser(Guid id)
        {
            // if not found return null
            return m_userRepository.FindById(id);
        }

        public async Task<FileboxUser> FindByIdAsyncUser(Guid id)
        {
            return await Task.Run(() => m_userRepository.FindByIdAsync(id));
        }


        public async Task<FileboxUser> FindUserByEmailAsync(string email)
        {
            return (await m_userRepository.FindByFilterAsync(user => user.Email == email)).FirstOrDefault();
        }
        public FileboxUser Update(FileboxUser user)
        {
            return m_userRepository.Update(user);
        }
    }
}
