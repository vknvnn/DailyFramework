using System;
using System.IO;
using Df.ContextBase;
using Df.JsonConfiguration;
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
        private readonly IJsonConfig _jsonConfig;



        public virtual DbSet<Tenant> Tenants { get; set; }

        public DfTestContext(ITenantFactory tenantFactory, IJsonConfig jsonConfig) : base(tenantFactory)
        {
            _jsonConfig = jsonConfig;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_jsonConfig.GetConnection("dftest"));
#if DEBUG
            LoggerFactory loggerDebugFactory = new LoggerFactory(new[] { new DebugLoggerProvider((category, level) => category == DbLoggerCategory.Database.Command.Name && level == LogLevel.Information) });
            optionsBuilder.UseLoggerFactory(loggerDebugFactory);
#endif
        }

    }
}
