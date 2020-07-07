using Regulus.RelationalTables.Raw;
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
            _Tables = new Dictionary<Type, Table>();
            _Build(_Sort(queryables));
        }

        private IEnumerable<ITableQueryable> _Sort(ITableQueryable[] queryables)
        {
            var sorter = new RelationalTables.Raw.RelationSorter(queryables.Select(q => q.GetTableType()));
            foreach(var t in sorter.Types)
            {
                yield return queryables.Where(q => q.GetTableType() == t).Single();
            }
        }

        private Dictionary<Type, Table> _Build(IEnumerable<Raw.ITableQueryable>  queryables)
        {
            var table = _Tables;
            foreach (var queryable in queryables)
            {
                table.Add(queryable.GetTableType() ,  _Create(queryable)); 
            }
            return table;
        }

        private Table _Create(Raw.ITableQueryable queryable)
        {
            var type = queryable.GetTableType();

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
            return (new object[0]).Cast<T>();
        }

        IEnumerable<object> ITableFindable.FindRows(Type type)
        {
            Table table;
            if (_Tables.TryGetValue(type, out table))
            {
                return table.Instances;
            }
            return (new object[0]);
        }
    }
}
