using System.Collections.Generic;

namespace Regulus.RelationalTables.Raw
{
    public interface IColumnProvidable
    {
        IEnumerable<Column> GetColumns();
    }
}
