using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Df.ContextBase.Base;

namespace Df.ContextBase.ChangeTracker
{
    public class AuditBase : EntityTenant
    {
        [Column("EntityId"), Required]
        public long EntityId { get; set; }
        [Column("ChangeTrackerDate"), Required]
        public DateTimeOffset ChangeTrackerDate { get; set; }
        [Column("TransactionId"), Required]
        public Guid TransactionId { get; set; }
        [Column("Stores")]
        public string Stores { get; set; }
        
    }
}
