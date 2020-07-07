using System.Collections.Generic;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public interface IRowQoeryable
        {
            IEnumerable<Column> GetColumns();
        }
    }
}
