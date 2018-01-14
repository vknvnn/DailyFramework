using Df.ContextBase.Base;
using System;
using System.Linq;
using System.Reflection;
using Df.ContextBase.ChangeTracker;
using Microsoft.EntityFrameworkCore;

namespace Df.ContextBase
{
    public partial class DataContext
    {
        private void AuditHandle()
        {
            var modifiedEntities = ChangeTracker.Entries<EntityTenant>()
                .Where(p => p.State == EntityState.Modified).ToList();

            foreach (var entry in modifiedEntities)
            {
                var entityType = entry.Context.Model.FindEntityType(entry.Entity.GetType());
                var audit = new DtoEntityAudit {EntityName = entityType.Name, EntityId = entry.Entity.Id };
                foreach (var prop in entry.OriginalValues.Properties)
                {
                    var originalValue = entry.OriginalValues[prop];
                    var currentValue = entry.CurrentValues[prop];
                    if (IsNotEqualAndExitAttr(originalValue, currentValue, prop.PropertyInfo))
                    {
                        audit.PropertyAudits.Add(new DtoPropertyAudit
                        {
                            PropertyName = prop.Name,
                            OldValue = originalValue,
                            NewValue = currentValue,
                        });
                    }
                }
                AuditList.Add(audit);
            }
        }

        private bool IsNotEqualAndExitAttr(object original, object current, PropertyInfo propInfo)
        {
            var attr = propInfo.CustomAttributes.FirstOrDefault(o => o.AttributeType == typeof(DfTrackingStore));
            if (attr == null)
            {
                return false;
            }

            var format = string.Empty;
            if (attr.ConstructorArguments[1].Value != null)
            {
                format = attr.ConstructorArguments[1].Value as string;
            }

            var type = attr.ConstructorArguments[0].Value as Type;
            if (type != null)
            {
                switch (type.Name)
                {
                    case "String":
                        return Equals(original.ToString().Trim(), current.ToString().Trim());
                    case "DateTime":
                        if (!string.IsNullOrEmpty(format))
                        {
                            var originalDateTime = (DateTime)original;
                            var currentDateTime = (DateTime)current;
                            return Equals(originalDateTime.ToString(format), currentDateTime.ToString(format));
                        }
                        break;
                    case "DateTimeOffset":
                        if (!string.IsNullOrEmpty(format))
                        {
                            var originalDateTime = (DateTimeOffset)original;
                            var currentDateTime = (DateTimeOffset)current;
                            return Equals(originalDateTime.ToString(format), currentDateTime.ToString(format));
                        }
                        break;

                }
            }
            return Equals(original, current);
        }

    }
}
