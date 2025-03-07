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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
