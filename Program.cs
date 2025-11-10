using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Config;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
 options.AddPolicy(name: "AllowFrontend",
 policy =>
 {
 policy.WithOrigins("http://localhost:4200")
 .AllowAnyHeader()
 .AllowAnyMethod();
 });
});

// Add services
builder.Services.AddControllers();
// registrar la infraestructura (DbContext, repos, servicios)
builder.Services.AddUsersInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
 var cfg = builder.Configuration;
 // parse key as base64 if possible, otherwise UTF8
 var keyConfig = cfg["Jwt:Key"] ?? string.Empty;
 byte[] keyBytes;
 try
 {
 keyBytes = Convert.FromBase64String(keyConfig);
 }
 catch
 {
 keyBytes = Encoding.UTF8.GetBytes(keyConfig);
 }

 options.RequireHttpsMetadata = false; // allow HTTP in development if needed
 options.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateLifetime = true,
 ValidateIssuerSigningKey = true,
 ValidIssuer = cfg["Jwt:Issuer"],
 ValidAudience = cfg["Jwt:Audience"],
 IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
 };

 options.Events = new JwtBearerEvents
 {
 OnAuthenticationFailed = ctx =>
 {
 var logger = ctx.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()?.CreateLogger("JwtBearer");
 logger?.LogError(ctx.Exception, "Authentication failed");
 return System.Threading.Tasks.Task.CompletedTask;
 },
 OnChallenge = context =>
 {
 var logger = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Logging.ILoggerFactory>()?.CreateLogger("JwtBearer");
 logger?.LogWarning("JwtBearer challenge: {0}", context.ErrorDescription);
 return System.Threading.Tasks.Task.CompletedTask;
 }
 };
 });

builder.Services.AddAuthorization();

var app = builder.Build();

// Enable middleware for Swagger in development
if (app.Environment.IsDevelopment())
{
 app.UseSwagger();
 app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
