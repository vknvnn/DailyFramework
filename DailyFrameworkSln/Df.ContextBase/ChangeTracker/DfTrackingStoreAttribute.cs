using System;

namespace Df.ContextBase.ChangeTracker
{
    public class DfTrackingStore: Attribute
    {
        public Type DataType { get; private set; }
        public string Format { get; private set; }
        public DfTrackingStore(Type type = null, string format = null)
        {
            DataType = type;
            Format = format;
        }
    }
}
