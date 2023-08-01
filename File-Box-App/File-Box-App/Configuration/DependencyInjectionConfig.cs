using RepositoryLib.Dal;
using RepositoryLib.Repository.Impl;
using RepositoryLib.Repository;
using FileBoxService.Service;
using Service.Services.DownloadService;
using Service.Services.FileServicePath;
using Service.Services.FolderService;
using Service.Services.ScanService;
using Service.Services.UploadService;

namespace File_Box_App.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddScoped<IFolderService, FolderService>();
            services.AddScoped<IUserLoginService, LoginService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IScanService, ScanService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IDownloadService, DownloadService>();
        }

        public static void ConfigureRepositoriesAndHelpers(this IServiceCollection services)
        {
            // helper classes
            services.AddSingleton<UserRepositoryDal>();
            services.AddSingleton<FolderRepositoryDal>();
            services.AddSingleton<FileRepositoryDal>();

            // Repositories
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IFileRepository, FileRepository>();
            services.AddSingleton<IFolderRepository, FolderRepository>();
        }

    }
}
