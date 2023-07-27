using File_Box_App.Service;
using RepositoryLib.Dal;
using RepositoryLib.Models;
using RepositoryLib.Repository;
using RepositoryLib.Repository.Impl;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<FileBoxDbContext>();
builder.Services.AddSingleton<FileBoxAppDal>();
builder.Services.AddScoped<IUserLoginService ,LoginService>();

// For added the DI to RepositoryLib Class Library
builder.Services.AddSingleton<IUserRepository ,UserRepository>();
builder.Services.AddSingleton<FileBoxAppDal>();
builder.Services.AddSingleton<IFileRepository ,FileRepository>();
builder.Services.AddSingleton<IFolderRepository ,FolderRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//builder.Services.AddEndpointsApiExplorer();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
