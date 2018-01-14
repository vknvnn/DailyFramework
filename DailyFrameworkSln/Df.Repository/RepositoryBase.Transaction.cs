using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Df.Repository
{
    public partial class RepositoryBase<TDbContext, TEntity>
    {
        public IDbContextTransaction BeginTransaction()
        {
            return Db.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return Db.Database.BeginTransactionAsync();
        }

        public void CommitTransaction()
        {
            Db.Database.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            Db.Database.RollbackTransaction();
        }
    }
}
