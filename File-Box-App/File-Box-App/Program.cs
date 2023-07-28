
using File_Box_App;

using FileBoxService.Service;
using RepositoryLib.Dal;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using RepositoryLib.Repository.Impl;
using Service.Services.FolderService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<FileBoxDbContext>();

builder.Services.AddScoped<IUserLoginService, LoginService>();

// For added the DI to RepositoryLib Class Library
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<UserRepositoryDal>();
builder.Services.AddSingleton<FolderRepositoryDal>();
builder.Services.AddSingleton<IFileRepository, FileRepository>();
builder.Services.AddSingleton<IFolderRepository, FolderRepository>();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IFolderService, FolderService>();
builder.Services.AddScoped<IUserLoginService, LoginService>();
builder.Services.AddControllers().AddApplicationPart(typeof(Presentation.Controllers.AssemblyReference).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureJwt(builder.Configuration);



var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
