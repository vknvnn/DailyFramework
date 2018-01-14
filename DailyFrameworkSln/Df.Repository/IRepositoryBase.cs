using System;
using System.Linq;
using System.Threading.Tasks;
using Df.ContextBase;
using Df.ContextBase.Base;
using Df.ContextBase.ChangeTracker;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Df.Repository
{
    public delegate void CrudMultiEntityInTransaction<in TDbContext>(TDbContext db, Guid transactionId, DelegateHandleChangeTracker handleChangeTracker) where TDbContext : DataContext;

    public interface IRepositoryBase<out TDbContext, TEntity>
        where TDbContext : DataContext
        where TEntity : EntityTracking
    {
        //Base
        TDbContext Db { get; }
        DbSet<TEntity> This { get; }
        DbSet<T> GetDbSet<T>() where T : EntityBase;

        //Transaction
        IDbContextTransaction BeginTransaction();
        Task<IDbContextTransaction> BeginTransactionAsync();
        void CommitTransaction();
        void RollbackTransaction();

        //CRUD
        int InsertAndCommit(TEntity entity);
        Task<int> InsertAndCommitAsync(TEntity entity);
        int UpdateAndCommit(TEntity entity);
        Task<int> UpdateAndCommitAsync(TEntity entity);
        int DeleteAndCommitWithTracking(long id);
        Task<int> DeleteAndCommitWithTrackingAsync(long id);
        int DeleteAndCommitForever(long id);
        Task<int> DeleteAndCommitForeverAsync(long id);

        /// <summary>
        /// Allow insert multi entities in a transaction
        /// </summary>
        /// <param name="action">this is a action that it's like pointer function.</param>
        /// <param name="handleChangeTracker">Audit handler</param>
        /// <returns>return a transactionId</returns>
        void CrudMultiEntitySingleTransaction(CrudMultiEntityInTransaction<TDbContext> action, DelegateHandleChangeTracker handleChangeTracker);

        //Query
        IQueryable<TEntity> ThisQuery();
        IQueryable<TEntity> Query();
        IQueryable<TEntity> QueryNoDf();
        TEntity GetById(long id);
    }
}
