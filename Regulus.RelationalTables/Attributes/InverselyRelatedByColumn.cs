using Regulus.RelationalTables.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.RelationalTables.Attributes
{
    public class InverselyRelatedByColumn : FieldParser
    {
        private readonly string _Column;

        public InverselyRelatedByColumn(string column)
        {
            this._Column = column;
        }
        public override object Parse(FieldInfo field, IEnumerable<Column> row, ITableable table)
        {
            var colVal = row.FirstOrDefault(c => c.Name == _Column).Value;
            if (colVal == null)
            {
                return null;
            }

            if (!field.FieldType.HasElementType)
                return _CreateOne(field, table, colVal);
            return _CreateArray(field, table, colVal);
        }

        private object _CreateOne(FieldInfo field, ITableable table, string colVal)
        {
            var elementType = field.FieldType;
            return table.FindRelatables(elementType).Where(r => r.Compare(colVal)).SingleOrDefault();            
        }

        private static object _CreateArray(FieldInfo field, ITableable table, string colVal)
        {
            var elementType = field.FieldType.GetElementType();
            var relatables = table.FindRelatables(elementType).Where(r => r.Compare(colVal)).ToArray();
            var length = relatables.Length;
            var val = Array.CreateInstance(elementType, length) as System.Array;
            for (int i = 0; i < length; i++)
            {
                val.SetValue(relatables[i], i);
            }
            return val;
        }
    }
}
