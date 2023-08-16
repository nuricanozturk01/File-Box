using File_Box_App.Configuration;
using FileBoxService.Service;
using RepositoryLib.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddSingleton<FileBoxDbContext>();

builder.Services.AddScoped<IUserLoginService, LoginService>();


/*
 * 
 * 
 * DI to RepositoryLib Class Library
 * Configurations with extension methods 
 * 
 * 
 */

// Repositories and Helper Classes
builder.Services.ConfigureRepositoriesAndHelpers(builder.Configuration);

// Services
builder.Services.ConfigureServices();

// Mapper
builder.Services.AddAutoMapper(typeof(Program));


/*
 * 
 * 
 * Configurations with extension methods 
 * 
 * 
 */


//JWT
builder.Services.ConfigureJwt(builder.Configuration);

// Max Request Configuration extension methods
builder.Services.ConfigureMaxRequest();

// Configure the file byte limits
builder.Services.ConfigureFormOptionMaxValues();

// Endpoint configures
builder.Services.AddControllers().AddApplicationPart(typeof(Presentation.Controllers.AssemblyReference).Assembly);
builder.Services.AddEndpointsApiExplorer();



/*
 * 
 * 
 * App Config with extension methods 
 * 
 * 
 */

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.ConfigurationCorsOptions();

app.Run();
