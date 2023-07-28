using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IFileRepository : ICrudRepository<FileboxFile, long>
    {
    }
}
