namespace Service.Services.ScanService
{
    public interface IScanService
    {
        Task<string> ScanUserAllFolders(Guid guid);
    }
}
