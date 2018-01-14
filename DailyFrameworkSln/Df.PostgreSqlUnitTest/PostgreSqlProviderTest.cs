using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Df.JsonConfiguration;
using Df.PostgreSqlUnitTest.PostgreSqlProvider;
using Df.PostgreSqlUnitTest.PostgreSqlProvider.Entities;
using Df.TenantBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Df.PostgreSqlUnitTest
{
    [TestClass]
    public class PostgreSqlProviderTest
    {
        private readonly Guid _tenant = new Guid("e66ae92f-7d05-4c61-baaf-2c49397c0a51");
        private readonly int _clientName = 420;
        private readonly string _userName = "nghiep.vo";
        private IServiceProvider _serviceProvider;
        public PostgreSqlProviderTest()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<DfTestContext>();
            serviceCollection.AddSingleton<IJsonConfig, JsonConfig>();
            serviceCollection.AddTransient<ITenantFactory>(s=> new TenantFactory(null, _tenant, _clientName, _userName));
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
        
        [TestMethod]
        public void InsertOrUpdate()
        {
            long idResutl = 0;
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<DfTestContext>();
                var objTenant = db.Tenants.FirstOrDefault(o => o.Id == 1);
                if (objTenant != null)
                {
                    objTenant.IsActive = !objTenant.IsActive;
                }
                else
                {
                    objTenant = new Tenant
                    {
                        IsActive = false,
                    };
                    db.Tenants.Add(objTenant);
                    
                }
                var result = db.SaveChangesAsync();
                result.Wait();
                idResutl = objTenant.Id;
            }
            Assert.AreEqual(idResutl, 1);
        }

        [TestMethod]
        public void AuditChange()
        {
            long idResutl = 0;
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<DfTestContext>();
                var objTenant = db.Tenants.FirstOrDefault(o => o.Id == 1);
                if (objTenant != null)
                {
                    objTenant.IsActive = !objTenant.IsActive;
                }
                var result = db.SaveChangesAsync();
                result.Wait();
                idResutl = db.AuditList.Count;
            }

            Assert.AreEqual(idResutl, 1);
        }
        [TestMethod]
        public void CheckTimeZone()
        {
            
            long idResutl = 0;
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var db = scope.ServiceProvider.GetService<DfTestContext>();

                var result = db.Tenants.FirstOrDefaultAsync(o => o.ModifiedDate  > DateTimeOffset.UtcNow.AddSeconds(-30));
                result.Wait();
                idResutl = result.Result.Id;
            }
            Assert.AreEqual(idResutl, 1);
        }
    }
}
