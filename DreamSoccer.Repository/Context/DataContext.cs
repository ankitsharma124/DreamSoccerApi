using DreamSoccer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace DreamSoccer.Repository.Context
{
    public class DataContext : DbContext
    {
        private static IConfiguration Configuration
        {
            get
            {
                var projectDirectory = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("bin", StringComparison.Ordinal));
                if (!File.Exists(Path.Combine(projectDirectory, "appsettings.json")))
                {
                    projectDirectory = Path.GetFullPath(Path.Combine(projectDirectory, @"..\DreamSoccerApi"));
                }
                IConfigurationBuilder builder = new ConfigurationBuilder()
                   .SetBasePath(projectDirectory)
                   .AddJsonFile("appsettings.json");

                return builder.Build();
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration.GetConnectionString("DefaultConnectionString"));

            base.OnConfiguring(optionsBuilder);
        }
        public DbSet<User> Users { get; set; }
    }
}