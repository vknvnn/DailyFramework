using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Df.ContextBase.Base
{
    public class EntityTracking : EntityTenant
    {
        [Column("CreatedBy"), Required]
        public string CreatedBy { get; set; }
        [Column("CreatedDate"), Required]
        public DateTimeOffset CreatedDate { get; set; }

        [Column("Version"), Required]
        public int Version { get; set; }
        [Column("ModifiedBy")]
        public string ModifiedBy { get; set; }
        [Column("ModifiedDate")]
        public DateTimeOffset? ModifiedDate { get; set; }

        [Column("IsDeleted"), Required]
        public bool IsDeleted { get; set; }
        [Column("DeletedBy")]
        public string DeletedBy { get; set; }
        [Column("DeletedDate")]
        public DateTimeOffset? DeletedDate { get; set; }
    }
}
