using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Df.ContextBase.Base;
using Df.ContextBase.ChangeTracker;

namespace Df.Repository
{
    public partial class RepositoryBase<TDbContext, TEntity>
    {
        public virtual void HandleChangeTracker(List<DtoEntityAudit> audits)
        {

        }

        protected Expression<Func<T, bool>> GetPredicateAudit<T>() where T : AuditBase
        {
            return o => o.TenantId.Equals(_tenantFactory.GetTenantId());
        }

        public IQueryable<T> QueryAudit<T>() where T : AuditBase
        {
            return GetDbSet<T>().Where(GetPredicateAudit<T>());
        }
    }
}
