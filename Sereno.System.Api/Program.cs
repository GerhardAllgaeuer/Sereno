using Sereno.System.Api.Config;
using Sereno.System.Api.Utilities;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Sereno Identity API", 
        Version = "v1",
        Description = "API for Sereno Identity Service"
    });
});

var configPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
    "Sereno",
    "System.Api",
    "appconfig.json"
);

Directory.CreateDirectory(Path.GetDirectoryName(configPath)!);
AppConfigService configService = new AppConfigService(configPath);
AppConfig config = configService.Load();

builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton(config);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sereno Identity API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
