using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Fansoft.DigitalTwin.Api.Extensions;

public static class AuthSetupExtension
{
    public static void AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"] ?? 
                                    throw new InvalidOperationException("AuthServer:Authority");
                options.Audience = configuration["AuthServer:Audience"] ?? 
                                   throw new InvalidOperationException("AuthServer:Audience");
                options.RequireHttpsMetadata = configuration.GetValue("AuthServer:RequireHttpsMetadata", false);
                
                // options.TokenValidationParameters = new TokenValidationParameters
                // {
                //     ValidateIssuer = true,
                //     ValidIssuer = "http://localhost:8090/realms/digitaltwin",
                // };
            });

        services.AddAuthorization();
    }
    
    public static void UseAuthConfigs(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }
}