using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Df.ContextBase.Base;
using Df.ContextBase.ChangeTracker;

namespace Df.PostgreSqlUnitTest.PostgreSqlProvider.Entities
{
    [Table("Tenant", Schema = "public")]
    public class Tenant : EntityTracking
    {
        [Column("IsActive"), Required, DfTrackingStore(typeof(bool))]
        public bool IsActive { get; set; }
    }
}
