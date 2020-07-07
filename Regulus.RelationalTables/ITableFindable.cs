using System;
using System.Collections.Generic;

namespace Regulus.RelationalTables
{
    public interface ITableFindable
    {
        IEnumerable<object> Find(Type fieldType);
    }
}