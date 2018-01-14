using System;
using System.Linq;
using System.Threading.Tasks;
using Df.ContextBase.Base;
using Df.ContextBase.ChangeTracker;
using Microsoft.EntityFrameworkCore;

namespace Df.Repository
{
    public partial class RepositoryBase<TDbContext, TEntity>
    {
        public void CrudMultiEntitySingleTransaction(CrudMultiEntityInTransaction<TDbContext> action, DelegateHandleChangeTracker handleChangeTracker)
        {
            var trans = BeginTransaction();
            try
            {
                action(Db, trans.TransactionId, handleChangeTracker);
                
                CommitTransaction();
            }
            catch (Exception)
            {
                RollbackTransaction();
                throw;
            }
        }
        #region Insert
        public int InsertAndCommit(TEntity entity)
        {
            This.Add(entity);
            Db.HandleChangeTracker = HandleChangeTracker;
            return Db.SaveChanges();
        }

        public Task<int> InsertAndCommitAsync(TEntity entity)
        {
            This.Add(entity);
            Db.HandleChangeTracker = HandleChangeTracker;
            return Db.SaveChangesAsync();
        }
        #endregion

        #region Update
        public int UpdateAndCommit(TEntity entity)
        {
            This.Update(entity);
            Db.HandleChangeTracker = HandleChangeTracker;
            return Db.SaveChanges();
        }

        public Task<int> UpdateAndCommitAsync(TEntity entity)
        {
            This.Update(entity);
            Db.HandleChangeTracker = HandleChangeTracker;
            return Db.SaveChangesAsync();
        }
        #endregion
        #region Delete

        public int DeleteAndCommitWithTracking(long id)
        {
            var entity = This.FirstOrDefault(o => o.Id == id);
            var entityTracking = entity as EntityTracking;
            if (entityTracking == null)
            {
                throw new Exception("This entity does not entity inherit entity traking");
            }
            entityTracking.IsDeleted = true;
            This.Update(entity);
            Db.HandleChangeTracker = HandleChangeTracker;
            return Db.SaveChanges();
        }

        public Task<int> DeleteAndCommitWithTrackingAsync(long id)
        {
            var entity = This.FirstOrDefaultAsync(o => o.Id == id);
            var entityTracking = entity.Result as EntityTracking;
            if (entityTracking == null)
            {
                throw new Exception("This entity does not entity inherit entity traking");
            }
            entityTracking.IsDeleted = true;
            This.Update(entity.Result);
            Db.HandleChangeTracker = HandleChangeTracker;
            return Db.SaveChangesAsync();
        }

        public int DeleteAndCommitForever(long id)
        {
            var entity = This.FirstOrDefault(o => o.Id == id);
            if (entity != null)
            {
                This.Remove(entity);
            }
            return Db.SaveChanges();
        }

        public Task<int> DeleteAndCommitForeverAsync(long id)
        {
            var entity = This.FirstOrDefaultAsync(o => o.Id == id);
            if (entity.Result != null)
            {
                This.Remove(entity.Result);
            }
            return Db.SaveChangesAsync();
        }
        #endregion
    }
}
