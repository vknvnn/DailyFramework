using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using LinqKit;

namespace Df.Repository
{
    public partial class RepositoryBase<TDbContext, TEntity>
    {
        #region all query type
        public IQueryable<TEntity> ThisQuery()
        {
            return This.Where(_predicate);
        }
        
        public IQueryable<TEntity> Query()
        {
            return GetDbSet<TEntity>().Where(_predicate);
        }

        public IQueryable<TEntity> QueryNoDf()
        {
            return GetDbSet<TEntity>();
        }
        #endregion

        public virtual TEntity GetById(long id)
        {
            var predicate = PredicateBuilder.New<TEntity>();
            predicate.Start(_predicate);
            var e = DynamicExpressionParser.ParseLambda(typeof(TEntity), typeof(bool),"Id = @0",id) as Expression<Func<TEntity, bool>>;
            predicate.And(e);
            //predicate.And(o => o.Id == id);
            return This.FirstOrDefault(_predicate);
        }
    }
}
