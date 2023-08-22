using ElmahCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RepositoryLib.Models;
using System.Text;

namespace File_Box_App.Configuration
{
    public static class ServiceConfig
    {





        public static void ConfigureMaxRequest(this IServiceCollection services)
        {
            services.Configure<IISServerOptions>(options => options.MaxRequestBodySize = int.MaxValue);
            services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = int.MaxValue);
            services.Configure<KestrelServerOptions>(options => options.Limits.MaxResponseBufferSize = 1024 * 1024 * 999);
        }






        public static void ConfigureFormOptionMaxValues(this IServiceCollection services)
        {
            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue;
            });
        }


        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FileBoxDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection")), ServiceLifetime.Transient);
        }
        public static void ConfigureElmah(this IServiceCollection services)
        {
            services.AddElmah();
        }

        public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>

            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["validIssuer"],
                    ValidAudience = jwtSettings["validAudience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            }
            );


        }
    }
}
