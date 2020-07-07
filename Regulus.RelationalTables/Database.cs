using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
    }
    
    public class Database : ITableFindable
    {
        readonly System.Collections.Generic.Dictionary<Type, Table> _Tables;
        public Database(params Raw.ITableQueryable[] queryables)
        {

            _Tables = _Create(queryables);
        }

        private Dictionary<Type, Table> _Create(Raw.ITableQueryable[] queryables)
        {
            var table = new Dictionary<Type, Table>();
            foreach (var queryable in queryables)
            {
                table.Add(queryable.GetType() ,  _Create(queryable)); 
            }
            return table;
        }

        private Table _Create(Raw.ITableQueryable queryable)
        {
            var type = queryable.GetType();

            var instances = new List<object>();
            var fields = type.GetFields();            
            foreach (var row in queryable.GetRows())
            {
                var instance = System.Activator.CreateInstance(type);
                foreach (var field in fields)
                {
                    var val = new FieldValue(field , row , this);
                    field.SetValue(instance, val.Instance);
                }
                instances.Add(instance);                
            }

            return new Table(instances);
        }

        public IEnumerable<T> Query<T>()
        {
            Table table;
            if(_Tables.TryGetValue(typeof(T), out table))
            {
                return table.Instances.Cast<T>();
            }
            return (new object[] { }).Cast<T>();
        }
    }
}
