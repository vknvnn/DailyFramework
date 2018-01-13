using System.IO;
using Df.ContextBase;
using Df.PostgreSqlUnitTest.PostgreSqlProvider.Entities;
using Df.TenantBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;

namespace Df.PostgreSqlUnitTest.PostgreSqlProvider
{
    public class DfTestContext : DataContext
    {
        public virtual DbSet<Tenant> Tenants { get; set; }
        public static IConfigurationRoot Configuration { get; set; }
        public DfTestContext(ITenantFactory tenantFactory) : base(tenantFactory)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            Configuration = builder.Build();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Configuration["connection:dftest"]);
#if DEBUG
            LoggerFactory loggerDebugFactory = new LoggerFactory(new[] { new DebugLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information) });
            optionsBuilder.UseLoggerFactory(loggerDebugFactory);
#endif
        }

    }
}
