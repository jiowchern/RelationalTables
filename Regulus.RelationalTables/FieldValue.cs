using Regulus.RelationalTables.Raw;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Regulus.RelationalTables
{
    public class FieldValue
    {
        private FieldInfo _Field;
        private IRowQoeryable _Row;
        private ITableFindable _Finder;

        public readonly object Instance;
        public FieldValue(FieldInfo field, IRowQoeryable row, ITableFindable findable)
        {
            this._Field = field;
            this._Row = row;
            this._Finder = findable;
            Instance = _Create();
        }

        private object _Create()
        {
            var fieldName = _Field.Name;
            var values = from col in _Row.GetColumns() where col.Name == fieldName select col.Value;
            var val = values.Single();
            var converter = System.ComponentModel.TypeDescriptor.GetConverter(_Field.FieldType);
            return converter.ConvertFromString(val);
        }
    }
}