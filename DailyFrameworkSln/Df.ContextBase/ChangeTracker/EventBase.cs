using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Df.ContextBase.Base;

namespace Df.ContextBase.ChangeTracker
{
    public class EventBase : EntityBase
    {
        [Column("Stores")]
        public string Stores { get; set; }
        [Column("ChangeTrackerDate"), Required]
        public DateTimeOffset ChangeTrackerDate { get; set; }
    }
}
