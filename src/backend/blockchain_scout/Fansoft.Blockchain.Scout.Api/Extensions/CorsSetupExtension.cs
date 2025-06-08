namespace Fansoft.Blockchain.Scout.Api.Extensions;

public static class CorsSetupExtension
{
    public static void AddCorsSetup(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins("http://localhost:5172") // em production, use the actual frontend URL
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
    
    public static void UseCorsSetup(this WebApplication app)
    {
        app.UseCors("AllowAllOrigins");
    }
}