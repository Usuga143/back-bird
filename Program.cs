using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Config;

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
 options.TokenValidationParameters = new TokenValidationParameters
 {
 ValidateIssuer = true,
 ValidateAudience = true,
 ValidateLifetime = true,
 ValidateIssuerSigningKey = true,
 ValidIssuer = cfg["Jwt:Issuer"],
 ValidAudience = cfg["Jwt:Audience"],
 IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"] ?? ""))
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
