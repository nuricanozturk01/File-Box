using Microsoft.EntityFrameworkCore;
using RepositoryLib.Models;

namespace File_Box_App
{
    public static class Config
    {
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
            => services.AddDbContext<FileBoxDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
    }
}
