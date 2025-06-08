namespace Fansoft.DigitalTwin.Api.Extensions;

public static class ApiSetupExtension
{
    public static void AddApiSetup(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Digital Twin API", Version = "v1" });
        });

        services.AddCoreDependencies(configuration);
        services.AddCorsSetup();
    }

    public static void UseApiSetup(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCoreDependencies();
        }
        
        app.UseCorsSetup();
        //app.UseHttpsRedirection();
        app.MapEventEndpoints();
    }
}