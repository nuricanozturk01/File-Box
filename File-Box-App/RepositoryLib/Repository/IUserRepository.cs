using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IUserRepository : ICrudRepository<FileboxUser, Guid>
    {






        /*
         * 
         * Update Users with given FileBoxUser parameter 
         * 
         * 
         */
        FileboxUser Update(FileboxUser user);
    }
}
