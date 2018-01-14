using System.Linq.Expressions;
using Df.ContextBase;
using Df.ContextBase.Base;
using Df.JsonConfiguration;
using Df.TenantBase;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Df.Repository
{
    public partial class RepositoryBase<TDbContext, TEntity>: IRepositoryBase<TDbContext, TEntity>
        where TDbContext : DataContext
        where TEntity : EntityTracking
    {
        private readonly IJsonConfig _jsonConfig;
        private readonly ITenantFactory _tenantFactory;
        private ExpressionStarter<TEntity> _predicate;
        public TDbContext Db { get; }
        public DbSet<TEntity> This { get; }

        public RepositoryBase(TDbContext db, IJsonConfig jsonConfig, ITenantFactory tenantFactory)
        {
            _jsonConfig = jsonConfig;
            _tenantFactory = tenantFactory;
            Db = db;
            This = db.Set<TEntity>();
            SetPredicate();
        }

        private void SetPredicate()
        {
            _predicate = PredicateBuilder.New<TEntity>();
            _predicate.Start(o => !o.IsDeleted);
            _predicate.And(o => o.TenantId.Equals(_tenantFactory.GetTenantId()));
        }

        public DbSet<T> GetDbSet<T>() where T: EntityBase
        {
            return Db.Set<T>();
        }

        
    }
}
