using Regulus.RelationalTables.Raw;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.RelationalTables.Attributes
{
    public class Merge : FieldParser
    {
        readonly string[] _Fields;
        public Merge(params string[] fields)
        {
            _Fields = fields;
        }
        public override object Parse(FieldInfo field, IEnumerable<Column> row, ITableable table)
        {
            var elementType = field.FieldType.GetElementType();
            var instance = System.Array.CreateInstance(elementType, _Fields.Length);

            for (int i = 0; i < _Fields.Length; i++)
            {
                var col = _Fields[i];
                var column = row.FirstOrDefault(c => c.Name == col);                
                var val = Regulus.Utility.ValueHelper.StringConvert(field.FieldType.GetElementType(), column.Value);
                instance.SetValue(val, i);
            }
            return instance;
        }
    }
}
