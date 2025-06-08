using Fansoft.Blockchain.Scout.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiSetup(builder.Configuration);

var app = builder.Build();

app.UseApiSetup();
await app.RunAsync();