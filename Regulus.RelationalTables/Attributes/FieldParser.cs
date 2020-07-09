using Regulus.RelationalTables.Raw;
using System.Collections.Generic;
using System.Reflection;

namespace Regulus.RelationalTables.Attributes
{
    public abstract class FieldParser : System.Attribute
    {
        public abstract object Parse(FieldInfo field, IEnumerable<Column> row, ITableable table);
    }
}
