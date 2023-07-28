using RepositoryLib.DTO;

namespace Service.Services.FolderService
{
    public interface IFolderService
    {
        Task<IEnumerable<FolderViewDto>> GetFoldersByUserIdAsync(Guid userId);
        Task<bool> CreateFolder(FolderSaveDTO folderSaveDto, FolderViewDto? folderViewDto);
        Task<bool> DeleteFolder(long folderId);
    }
}
