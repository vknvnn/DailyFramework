using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Df.ContextBase.Base
{
    public class EntityBase
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        /// <summary>
        /// When set this property is true, that mean: the entity consitant with DB by ORM
        /// </summary>
        [NotMapped]        
        public bool IsNoneTracking { get; set; }

        
    }
}
