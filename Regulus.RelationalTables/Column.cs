using Regulus.RelationalTables.Raw;
using System;
using System.Linq;
using System.Reflection;

namespace Regulus.RelationalTables
{
    public class Array : System.Attribute
    {
        public readonly string[] Fields;
        public Array(params string[] fields)
        {
            Fields = fields;
        }

        internal object Create(FieldInfo field, IRowQueryable row)
        {
            var instance = System.Array.CreateInstance(field.FieldType.GetElementType(), Fields.Length);

            for (int i = 0; i < Fields.Length; i++)
            {                
                var col = Fields[i];
                var column = row.GetColumns().FirstOrDefault(c => c.Name == col);
                var val = Regulus.Utility.ValueHelper.StringConvert(field.FieldType.GetElementType(), column.Value);
                instance.SetValue(val, i);
            }
            return instance;
        }
    }
    namespace Raw
    {
        public struct Column
        {
            public readonly string Name;
            public readonly string Value;
            public Column(string name , string value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
