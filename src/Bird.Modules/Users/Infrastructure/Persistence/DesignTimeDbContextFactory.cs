using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
    {
        public UsersDbContext CreateDbContext(string[] args)
        {
            // EF tools run with working directory set to the project folder, so appsettings.json should be available
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("Default") ?? "Data Source=backbird.db";

            var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new UsersDbContext(optionsBuilder.Options);
        }
    }
}
