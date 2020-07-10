using Regulus.RelationalTables.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Regulus.RelationalTables.Attributes
{
    public class InverselyRelated : FieldParser
    {
        public override object Parse(FieldInfo field, IEnumerable<Column> row, ITableable table)
        {
            var col = row.FirstOrDefault( c=>c.Name == field.Name);
            if(col.Value == null)
            {
                return null;
            }
            if (!field.FieldType.HasElementType)
                return false;
            var elementType = field.FieldType.GetElementType();
            var relatables = table.FindRelatables(elementType).Where(r => r.Compare(col.Value)).ToArray();
            var length = relatables.Length;
            var val = Array.CreateInstance(elementType, length) as System.Array;
            for (int i = 0; i < length; i++)
            {
                val.SetValue(relatables[i],i);
            }
            return val;
        }
    }
}
