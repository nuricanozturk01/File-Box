using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IUserRepository : ICrudRepository<FileboxUser, Guid>
    {

    }
}
