using Regulus.RelationalTables.Attributes;
using Regulus.RelationalTables.Raw;
using System;
using System.Linq;
using System.Reflection;


namespace Regulus.RelationalTables
{

    public class FieldValue
    {
        private readonly FieldInfo _Field;
        private readonly IColumnProvidable _Row;
        private readonly ITableable _Finder;

        public readonly object Instance;
        public FieldValue(FieldInfo field, IColumnProvidable row, ITableable findable)
        {

            this._Field = field;
            this._Row = row;
            this._Finder = findable;
            Instance = _Create();
        }

        private object _Create()
        {
            var parser = _Field.GetCustomAttribute<Regulus.RelationalTables.Attributes.FieldParser>(true);
            if(parser !=null)
            {
                return parser.Parse(_Field, _Row.GetColumns(), _Finder);
            }
            object instance;
            if (_TryRelation(out instance))
                return instance;
            return _Default();
        }

       

        private bool _TryRelation(out object instance)
        {
            instance = null;
            if (!_Field.FieldType.GetInterfaces().Where(i => i == typeof(IRelatable)).Any())
                return false;
            var rows = from row in _Finder.FindRelatables(_Field.FieldType) select row;
            if (!rows.Any())
                return false;

            var colValue = (from col in _Row.GetColumns() where col.Name == _Field.Name select col.Value).Single();
            var relatableRows = from relatable in rows                       
                       where relatable.Compare(colValue)
                       select relatable;

            try
            {
                instance = relatableRows.Single();
                return true;
            }
            catch (System.InvalidOperationException ioe)
            {               
                throw new Exception($"No related type was found. Type:{_Field.DeclaringType.FullName} Related Type:{_Field.FieldType.FullName} Key:{colValue}",ioe );
            }
            
        }

        private object _Default()
        {
            

            var fieldName = _Field.Name;
            var values = from col in _Row.GetColumns() where col.Name == fieldName select col.Value;
            var value = values.FirstOrDefault();
            if (value == null)
                value = string.Empty;
            return Regulus.Utility.ValueHelper.StringConvert(_Field.FieldType, value);            
            
        }
    }
}