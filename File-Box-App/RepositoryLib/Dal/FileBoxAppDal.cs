using Microsoft.EntityFrameworkCore;
using RepositoryLib.Models;
using RepositoryLib.Repository;

namespace RepositoryLib.Dal
{
    public class FileBoxAppDal
    {
        private readonly IUserRepository m_userRepository;
        private readonly IFileRepository m_fileRepository;
        private readonly IFolderRepository m_folderRepository;

        public FileBoxAppDal(IUserRepository userRepository, 
                            IFileRepository fileRepository, 
                            IFolderRepository folderRepository)
        {
            m_userRepository = userRepository;
            m_fileRepository = fileRepository;
            m_folderRepository = folderRepository;
        }

        public FileboxFolder Save(FileboxFolder t)
        {
            return m_folderRepository.Save(t);
        }
        public Task<IEnumerable<FileboxFile>> FindAllFilesAsync()
        {
            return m_fileRepository.FindAllAsync();
        }

        public void InsertFolder(long parentFolder, Guid userId, string folderName, string folderPath)
        {
            m_folderRepository.InsertFolder(parentFolder, userId, folderName, folderPath);
        }

    }
}
