using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinhaCarteira.API.Data;
using MinhaCarteira.API.Services;
using MinhaCarteira.API.Repositories;
using MinhaCarteira.API.Filters;
using System.Text;
using Swashbuckle.AspNetCore.SwaggerGen;
using Scalar.AspNetCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using System.IO;
using MinhaCarteira.API.Tranformers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
    options.OpenApiVersion = Microsoft.OpenApi.OpenApiSpecVersion.OpenApi3_0;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000",     // React default
                "http://localhost:5173",     // Vite default
                "http://localhost:4200",     // Angular default
                "http://localhost:8080",     // Vue default
                "https://localhost:3000",
                "https://localhost:5173",
                "https://localhost:4200",
                "https://localhost:8080"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")))
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Minha Carteira API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.OperationFilter<AuthorizeCheckOperationFilter>();
    c.SchemaFilter<SwaggerEnumFilter>();

});

var app = builder.Build();

app.UseCors("AllowFrontend");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "Minha Carteira API v1");
    });
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseReDoc(o =>
    {
        o.RoutePrefix = "redoc";
        o.SpecUrl = "/openapi/v1.json";
        o.DocumentTitle = "Minha Carteira Api";
    });
}

if (!app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("UseHttps"))
{
    app.UseHttpsRedirection();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations and seed data at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
    await DbSeeder.SeedData(db);
}

app.Run();



