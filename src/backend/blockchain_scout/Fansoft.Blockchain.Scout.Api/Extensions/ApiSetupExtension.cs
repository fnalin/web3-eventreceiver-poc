using Fansoft.Blockchain.Scout.Api.Endpoints;

namespace Fansoft.Blockchain.Scout.Api.Extensions;

public static class ApiSetupExtension
{
    public static void AddApiSetup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Blockchain Scout API", Version = "v1" });
        });

        services.AddDependencies(configuration);
        services.AddCorsSetup();
    }

    public static void UseApiSetup(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseDependencies();
        }
        
        app.UseCorsSetup();
        //app.UseHttpsRedirection();
        //app.UseAuthConfigs();
        app.MapEndpoints();
    }
}