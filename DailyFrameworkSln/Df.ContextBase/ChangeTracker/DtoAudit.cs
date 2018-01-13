using System;
using System.Collections.Generic;
using System.Text;

namespace Df.ContextBase.ChangeTracker
{
    public class DtoEntityAudit
    {
        public DtoEntityAudit()
        {
            PropertyAudits = new List<DtoPropertyAudit>();
        }
        public string EntityName { get; set; }
        public long EntityId { get; set; }
        public List<DtoPropertyAudit> PropertyAudits { get; set; }
    }
    public class DtoPropertyAudit
    {
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
    }
}
