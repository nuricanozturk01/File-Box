using File_Box_App;

using FileBoxService.Service;
using RepositoryLib.Dal;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using RepositoryLib.Repository.Impl;
using Service.Services.FileServicePath;
using Service.Services.FolderService;

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

// Mapper
builder.Services.AddAutoMapper(typeof(Program));

//JWT
builder.Services.ConfigureJwt(builder.Configuration);

builder.Services.AddControllers().AddApplicationPart(typeof(Presentation.Controllers.AssemblyReference).Assembly);
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
