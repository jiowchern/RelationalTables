using System;
using System.Collections.Generic;

namespace Regulus.RelationalTables.Raw
{
    public interface IRowProvidable
    {
        Type GetTableType();
        IEnumerable<IColumnProvidable> GetRows();
    }
}
