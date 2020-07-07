using System;
using System.Collections.Generic;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public interface ITableQueryable
        {
            Type GetType();
            IEnumerable<IRowQueryable> GetRows();
        }
    }
    
}
