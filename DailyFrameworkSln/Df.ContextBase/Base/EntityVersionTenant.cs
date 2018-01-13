using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Df.ContextBase.Base
{
    public class EntityTenant : EntityBase
    {
        [Column("TenantId")]
        public Guid TenantId { get; set; }
    }
}
