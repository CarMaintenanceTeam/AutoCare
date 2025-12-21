using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace AutoCare.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "../AutoCare.Api/appsettings.json"))
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            // Create dummy services for interceptors
            var auditInterceptor = new Interceptors.AuditableEntityInterceptor();
            var eventInterceptor = new Interceptors.DomainEventDispatcherInterceptor();

            optionsBuilder.AddInterceptors(auditInterceptor, eventInterceptor);

            return new ApplicationDbContext(optionsBuilder.Options, auditInterceptor, eventInterceptor);
        }
    }
}