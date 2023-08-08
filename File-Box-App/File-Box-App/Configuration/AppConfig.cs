namespace File_Box_App.Configuration
{
    public static class AppConfig
    {
        public static void ConfigurationCorsOptions(this IApplicationBuilder app) 
        {
            app.UseCors(
                builder => builder
                    .WithOrigins("http://localhost:5299", "http://localhost:3000", "http://localhost:5299")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                );
        }
    }
}
