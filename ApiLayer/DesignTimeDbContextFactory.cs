using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ApiLayer
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(
                "Server=localhost;Database=farmmanagement;User=root;Password=12345678;",
                ServerVersion.AutoDetect("Server=localhost;Database=farmmanagement;User=root;Password=12345678;")
            );

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
