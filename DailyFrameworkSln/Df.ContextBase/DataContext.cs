using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Df.ContextBase.Base;
using Df.ContextBase.ChangeTracker;
using Df.TenantBase;
using Microsoft.EntityFrameworkCore;

namespace Df.ContextBase
{
    public partial class DataContext : DbContext
    {
        private readonly ITenantFactory _tenantFactory;
        private List<DtoEntityAudit> _auditList;
        public DelegateHandleChangeTracker HandleChangeTracker { get; set; }
        public DataContext(ITenantFactory tenantFactory)
        {
            _tenantFactory = tenantFactory;
            _auditList = new List<DtoEntityAudit>();
        }

        /// <summary>
        /// Tracking self entity base
        /// </summary>
        private void HandleTracking()
        {
            foreach (var entry in ChangeTracker.Entries<EntityTenant>())
            {
                if (entry.Entity.IsNoneTracking)
                {
                    continue;
                }
                EntityTracking entity;
                switch (entry.State)
                {
                    case EntityState.Detached:
                    case EntityState.Modified:
                        entity = entry.Entity as EntityTracking;
                        if (entity != null)
                        {
                            entity.Version++;
                            entity.ModifiedDate = _tenantFactory.GetCurrentClientTime();
                            entity.ModifiedBy = _tenantFactory.GetUserName();
                        }


                        break;
                    case EntityState.Added:
                        entry.Entity.TenantId = _tenantFactory.GetTenantId();
                        entity = entry.Entity as EntityTracking;
                        if (entity != null)
                        {
                            entity.Version = 0;
                            entity.CreatedDate = _tenantFactory.GetCurrentClientTime();
                            entity.CreatedBy = _tenantFactory.GetUserName();
                        }
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }                
            }            
        }

        public override int SaveChanges()
        {
            HandleTracking();
            AuditHandle();
            HandleChangeTracker?.Invoke(_auditList);
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            HandleTracking();
            AuditHandle();
            HandleChangeTracker?.Invoke(_auditList);
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
