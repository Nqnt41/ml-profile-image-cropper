using ProfileSearch.Server.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connectionString = builder.Configuration["connectionString"];
var cloudinaryUrl = builder.Configuration["Cloudinary:url"];
var cloudinaryUploadPreset = builder.Configuration["Cloudinary:uploadPreset"];

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No connectionString found, set it as connectionString in the secrets storage.");
}

if (string.IsNullOrEmpty(cloudinaryUrl))
{
    throw new InvalidOperationException("No cloudinaryUrl found, set it as cloudinaryUrl in the secrets storage.");
}

if (string.IsNullOrEmpty(cloudinaryUploadPreset))
{
    throw new InvalidOperationException("No cloudinaryUploadPreset found, set it as cloudinaryUploadPreset in the secrets storage.");
}

builder.Services.AddDbContext<ProfileSearchContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddSingleton(new CloudinaryContext(cloudinaryUrl, cloudinaryUploadPreset));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
