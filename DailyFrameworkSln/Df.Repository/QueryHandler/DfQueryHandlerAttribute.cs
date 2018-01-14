using System;
using System.Collections.Generic;
using System.Text;

namespace Df.Repository.QueryHandler
{
    public class DfQueryHandlerAttribute: Attribute
    {
        private string _entityName;
        public DfQueryHandlerAttribute(string entityName = null)
        {
            _entityName = entityName;
        }
    }
}
