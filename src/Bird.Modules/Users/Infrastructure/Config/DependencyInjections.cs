using BackBird.Api.src.Bird.Modules.Users.Domain.Interfaces;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BackBird.Api.src.Bird.Modules.Users.Infrastructure.Config
{
    public static class DependencyInjections 
    {
        public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UsersDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("Default")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
