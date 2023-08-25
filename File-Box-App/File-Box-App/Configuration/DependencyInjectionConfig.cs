using ElmahCore.Mvc;
using FileBoxService.Service;
using FileBoxTest.LoginTest;
using RepositoryLib.Dal;
using RepositoryLib.Repository;
using Service.Services.DownloadService;
using Service.Services.EmailService;
using Service.Services.FileServicePath;
using Service.Services.FolderService;
using Service.Services.ForgottenInformationService;
using Service.Services.ScanService;
using Service.Services.TokenService;
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
            services.AddScoped<IForgottenInformationService, ForgottenInformationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITokenService, TokenService>();
        }






        public static void ConfigureRepositoriesAndHelpers(this IServiceCollection services, IConfiguration configuration)
        {
            // helper classes
            services.AddScoped<UserRepositoryDal>();
            services.AddScoped<FolderRepositoryDal>();
            services.AddScoped<FileRepositoryDal>();
            services.AddElmah();
            services.AddHttpContextAccessor();

            // Repositories
            services.AddScoped(typeof(IGenericRepository<,>), typeof(CrudRepository<,>));
        }

    }
}