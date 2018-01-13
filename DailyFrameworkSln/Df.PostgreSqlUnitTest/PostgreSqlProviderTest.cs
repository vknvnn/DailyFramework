using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Df.PostgreSqlUnitTest.PostgreSqlProvider;
using Df.PostgreSqlUnitTest.PostgreSqlProvider.Entities;
using Df.TenantBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Df.PostgreSqlUnitTest
{
    [TestClass]
    public class PostgreSqlProviderTest
    {
        private readonly ITenantFactory _tenantFactory;
        private readonly Guid _tenant = new Guid("e66ae92f-7d05-4c61-baaf-2c49397c0a51");
        private readonly int _clientName = 420;
        private readonly string _userName = "nghiep.vo";
        public PostgreSqlProviderTest()
        {
            _tenantFactory = new TenantFactory(null, _tenant, _clientName, _userName);
        }
        
        [TestMethod]
        public void InsertOrUpdate()
        {
            long idResutl = 0;
            using (var db = new DfTestContext(_tenantFactory))
            {
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
                db.SaveChanges();
                
                idResutl = objTenant.Id;
            }
            Assert.AreEqual(idResutl, 1);
        }

        [TestMethod]
        public void AuditChange()
        {
            long idResutl = 0;
            using (var db = new DfTestContext(_tenantFactory))
            {
                var objTenant = db.Tenants.FirstOrDefault(o => o.Id == 1);
                if (objTenant != null)
                {
                    objTenant.IsActive = !objTenant.IsActive;
                }

                db.SaveChanges();
                idResutl = db.AuditList.Count;
            }
            Assert.AreEqual(idResutl, 1);
        }
        [TestMethod]
        public void CheckTimeZone()
        {
            long idResutl = 0;
            using (var db = new DfTestContext(_tenantFactory))
            {
                var objTenant = db.Tenants.FirstOrDefault(o => o.ModifiedDate > DateTimeOffset.UtcNow.AddSeconds(-30));
                if (objTenant != null)
                {
                    idResutl = objTenant.Id;
                }
                
            }
            Assert.AreEqual(idResutl, 1);
        }
    }
}
