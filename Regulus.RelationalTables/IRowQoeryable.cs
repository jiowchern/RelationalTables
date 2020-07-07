using System.Collections.Generic;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public interface IRowQueryable
        {
            IEnumerable<Column> GetColumns();
        }
    }
}
