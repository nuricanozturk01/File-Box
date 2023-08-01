using File_Box_App;

using FileBoxService.Service;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using RepositoryLib.Dal;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using RepositoryLib.Repository.Impl;
using Service.Services.DownloadService;
using Service.Services.FileServicePath;
using Service.Services.FolderService;
using Service.Services.ScanService;
using Service.Services.UploadService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<FileBoxDbContext>();

builder.Services.AddScoped<IUserLoginService, LoginService>();

// For added the DI to RepositoryLib Class Library

// helper classes
builder.Services.AddSingleton<UserRepositoryDal>();
builder.Services.AddSingleton<FolderRepositoryDal>();
builder.Services.AddSingleton<FileRepositoryDal>();

// Repositories
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IFileRepository, FileRepository>();
builder.Services.AddSingleton<IFolderRepository, FolderRepository>();

// Services
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IUserLoginService, LoginService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IScanService, ScanService>();
builder.Services.AddScoped<IUploadService, UploadService>();
builder.Services.AddScoped<IDownloadService, DownloadService>();

// Mapper
builder.Services.AddAutoMapper(typeof(Program));

//JWT
builder.Services.ConfigureJwt(builder.Configuration);

// Configure the file byte limits

builder.Services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue;
});
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});
 
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue; // if don't set default value is: 30 MB
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    // Set the maximum response buffer size to a larger value (in bytes).
    options.Limits.MaxResponseBufferSize = 1024 * 1024 * 999; // For example, 100 MB
});



// Endpoint configures
builder.Services.AddControllers().AddApplicationPart(typeof(Presentation.Controllers.AssemblyReference).Assembly);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();
app.UseCors(
                builder => builder
                    .WithOrigins(
                        "http://localhost:5299",
                        "http://127.0.0.1:5500", // Live Server
                        "http://localhost:5299")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
app.Run();
