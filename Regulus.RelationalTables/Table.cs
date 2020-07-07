using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables
{
    internal class Table
    {
        
        public readonly object[] Instances;

        public Table(IEnumerable<object> instances)
        {        
            this.Instances = instances.ToArray();
        }
    }
}