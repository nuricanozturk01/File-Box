using RepositoryLib.Models;

namespace RepositoryLib.Repository
{
    public class UnitOfWork : IDisposable
    {
        private FileBoxDbContext m_dbContext = new FileBoxDbContext();
        private IGenericRepository<FileboxUser, Guid> m_userRepository;
        private IGenericRepository<FileboxFile, long> m_fileRepository;
        private IGenericRepository<FileboxFolder, long> m_folderRepository;
        private bool disposed = false;

        public FileBoxDbContext Context => m_dbContext;
        public IGenericRepository<FileboxUser, Guid> UserRepository
        {
            get
            {
                return m_userRepository is null ? new CrudRepository<FileboxUser, Guid>(m_dbContext) : m_userRepository;
            }
        }

        public IGenericRepository<FileboxFile, long> FileRepository
        {
            get
            {
               return m_fileRepository is null ? new CrudRepository<FileboxFile, long>(m_dbContext) : m_fileRepository;
            }
        }
        public IGenericRepository<FileboxFolder, long> FolderRepository
        {
            get
            {
                return m_folderRepository is null ? new CrudRepository<FileboxFolder, long>(m_dbContext) : m_folderRepository;
            }
        }
        public void Save()
        {
            m_dbContext.SaveChanges();
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
                if (disposing)
                  m_dbContext.Dispose();

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
