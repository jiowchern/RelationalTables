using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables
{
    public class Table
    {        
        public readonly object[] Instances;
        public readonly Type Type;
        public Table(Type type,IEnumerable<object> instances)
        {
            Type = type;
            this.Instances = instances.ToArray();
        }
    }
}