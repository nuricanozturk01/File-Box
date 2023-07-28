using AutoMapper;
using RepositoryLib.Dal;
using RepositoryLib.DTO;
using RepositoryLib.Models;

namespace Service.Services.FolderService
{
    public class FolderService : IFolderService
    {
        private readonly FolderRepositoryDal m_folderDal;
        private readonly UserRepositoryDal m_userRepositoryDal;
        private readonly IMapper m_mapper;

        public FolderService(FolderRepositoryDal folderDal, IMapper mapper, UserRepositoryDal userRepositoryDal)
        {
            m_folderDal = folderDal;
            m_mapper = mapper;
            m_userRepositoryDal = userRepositoryDal;
        }

        public async Task<bool> CreateFolder(FolderSaveDTO folderSaveDto, FolderViewDto? folderViewDto)
        {
            
            if (folderViewDto is not null)
            {
                long? pid = -1;
                try
                {
                    pid  = folderViewDto.parentFolderId;
                }
                catch {
                    pid = null;
                }

                var user = await m_userRepositoryDal.FindByIdAsyncUser(Guid.Parse(folderViewDto.userId));
                var fullName = folderViewDto.folderPath + "\\" + folderSaveDto.folderName;
                var fileBoxFolder = new FileboxFolder(folderViewDto.parentFolderId == null ? null : folderViewDto.parentFolderId, Guid.Parse(folderViewDto.userId), folderSaveDto.folderName, user.Username);
                m_folderDal.Save(fileBoxFolder);
                Directory.CreateDirectory(fullName);
                return true;
            }

            return false;                       
            //FileOperationUtil.CreateFileIfNotExists(folderSaveDto.folderPath);
           /* var f = new FileboxFolder(null, Guid.Parse("5C69A30A-0B56-4A4D-A777-6B3656B14264"), "deneme", "asdas");
           var folder =  await m_folderDal.SaveAsync(f);
            return true;*/
        }

        public async Task<bool> DeleteFolder(long folderId)
        {
            var dir = await m_folderDal.FindByIdAsync(folderId);
            m_folderDal.Delete(dir);
            Directory.Delete(Util.DIRECTORY_BASE +  dir.FolderPath + "\\" + dir.FolderName, true);
            return true;
        }

        public async Task<IEnumerable<FolderViewDto>> GetFoldersByUserIdAsync(Guid userId)
        {
            var folders = m_folderDal.AllFolders.Where(x => x.UserId == userId).ToList();
            var folderViewDtos = folders.Select(folder => m_mapper.Map<FolderViewDto>(folder)).ToList();
            return await Task.FromResult(folderViewDtos);
        }

 
    }
}
