using Sereno.System.Api.Config;
using Sereno.System.Api.Utilities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Service Configuration
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
