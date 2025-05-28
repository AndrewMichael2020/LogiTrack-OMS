using LogiTrack.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using System.IO;

// Add this at the top if you want to load .env variables automatically
try
{
    DotNetEnv.Env.Load();
}
catch { /* Ignore if DotNetEnv is not installed or .env not found */ }

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(), // This works in all environments
    Args = args
});

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
// Ensure Swashbuckle.AspNetCore is installed:
builder.Services.AddSwaggerGen();

// Enable in-memory caching
builder.Services.AddMemoryCache();

// Always register the production provider here
// Remove any previous AddDbContext<LogiTrackContext> registration above this block

// Only register the provider once, based on environment variable
var useInMemory = Environment.GetEnvironmentVariable("USE_INMEMORY_DB") == "1";
if (useInMemory)
{
    builder.Services.AddDbContext<LogiTrack.Models.LogiTrackContext>(options =>
        options.UseInMemoryDatabase("TestDb"));
}
else
{
    var dbPath = Environment.GetEnvironmentVariable("ASPNETCORE_TEST_DB") ?? "logitrack.db";
    builder.Services.AddDbContext<LogiTrack.Models.LogiTrackContext>(options =>
        options.UseSqlite($"Data Source={dbPath}"));
}

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LogiTrackContext>();

// Add JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "logitrack",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "logitrack",
        // Use a fallback key of at least 16 characters (128 bits)
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Key"] ?? "logitrack_super_secret_key!"))
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); // Add this before UseAuthorization
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }