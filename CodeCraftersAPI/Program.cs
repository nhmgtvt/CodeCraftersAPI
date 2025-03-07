using CodeCrafters.Application.Services.Auth;
using CodeCrafters.Domain.Interfaces.Auth;
using CodeCrafters.Infrastructure.Persistence;
using CodeCrafters.Infrastructure.Repositories;
using CodeCrafters.Infrastructure.Services.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register JWT settings and provider
builder.Services.AddSingleton<IJwtProvider, JwtProvider>();
// Register application services (Use Cases)
builder.Services.AddScoped<RegisterService>();
builder.Services.AddScoped<PasswordLoginService>();
builder.Services.AddScoped<OAuthLoginService>();

// Configure the database context for SQLite
// Fetch database settings from configuration
var dbSettings = builder.Configuration.GetSection("DbSettings");

// Set defaults for database settings if they are missing
var databaseFileName = dbSettings.GetValue<string>("DatabaseFileName") ?? "data.db";  // Default to "data.db" if not provided
var databaseDirectory = dbSettings.GetValue<string>("DatabaseDirectory") ?? "./";    // Default to current directory if not provided

// Combine the directory and file name to get the full path
var dbPath = Path.Combine(Directory.GetCurrentDirectory(), databaseDirectory, databaseFileName);

// Add DbContext with dynamically created connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = $"Data Source={dbPath}";
    options.UseSqlite(connectionString);
});


// Add DbContext with dynamically created connection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = $"Data Source={dbPath}";
    options.UseSqlite(connectionString);
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();

// Register external OAuth service
builder.Services.AddHttpClient<IOAuthService, OAuthService>();

// Add authentication and authorization
var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing.");
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Configure logging (if needed)
builder.Services.AddLogging();

// Configure Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure the database and tables are created automatically
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
