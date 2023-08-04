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
        private readonly IGenericRepository<FileboxUser, Guid> m_userRepository;

        public UserRepositoryDal(IGenericRepository<FileboxUser, Guid> userRepository)
        {
            m_userRepository = userRepository;
        }






        public async Task<IEnumerable<FileboxUser>> FindByFilterAsyncUser(Expression<Func<FileboxUser, bool>> predicate)
        {
            return await Task.Run(() => m_userRepository.FindByFilterAsync(predicate));
        }






        public async Task<FileboxUser> FindByIdAsyncUser(Guid id)
        {
            return await Task.Run(() => m_userRepository.FindByIdAsync(id));
        }







        public async Task<FileboxUser> FindUserByEmailAsync(string email)
        {
            return (await m_userRepository.FindByFilterAsync(user => user.Email == email)).FirstOrDefault();
        }






        public void SaveChanges()
        {
            m_userRepository.SaveChanges();
        }






        public FileboxUser Update(FileboxUser user)
        {
           return m_userRepository.Update(user);
        }






        public async Task<FileboxUser?> FindUserByUsername(string username)
        {
            return m_userRepository.FindByPredicate(user => user.Username == username);
            
        }






        public async Task SaveChangesAsync()
        {
            await m_userRepository.SaveChangesAsync();
        }
    }
}
