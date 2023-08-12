namespace File_Box_App.Configuration
{
    public static class AppConfig
    {
        public static void ConfigurationCorsOptions(this IApplicationBuilder app) 
        {
            app.UseCors(
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                   // .AllowCredentials()
                );
        }
    }
}
