using System;
using System.Collections.Generic;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public interface ITableQueryable
        {
            Type GetTableType();
            IEnumerable<IRowQueryable> GetRows();
        }
    }
    

}
