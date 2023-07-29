using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public interface IFileRepository : ICrudRepository<FileboxFile, long>
    {
        void RemoveAllAsync(IEnumerable<FileboxFile> files);
        FileboxFile Update(FileboxFile file);
    }
}
