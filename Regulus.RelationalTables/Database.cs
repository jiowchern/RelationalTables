using Regulus.RelationalTables.Raw;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables.Serialization
{
}
namespace Regulus.RelationalTables
{


    public class Database : ITableable
    {
        readonly System.Collections.Generic.Dictionary<Type, Table> _Tables;
        public readonly IReadOnlyDictionary<Type, Table> Tables;
        public Database(params Raw.IRowProvidable[] queryables)
        {
            _Tables = new Dictionary<Type, Table>();
            Tables = _Tables;
            _Build(_Sort(queryables));
        }

        private IEnumerable<IRowProvidable> _Sort(IRowProvidable[] queryables)
        {
            var sorter = new RelationalTables.Raw.RelationSorter(queryables.Select(q => q.GetTableType()));
            foreach(var t in sorter.Types)
            {
                yield return queryables.Where(q => q.GetTableType() == t).Single();
            }
        }

        private Dictionary<Type, Table> _Build(IEnumerable<Raw.IRowProvidable>  queryables)
        {
            var table = _Tables;
            foreach (var queryable in queryables)
            {
                table.Add(queryable.GetTableType() ,  _Create(queryable)); 
            }
            return table;
        }

        private Table _Create(Raw.IRowProvidable queryable)
        {
            var type = queryable.GetTableType();

            var instances = new List<object>();
            var fields = type.GetFields();            
            foreach (var row in queryable.GetRows())
            {
                var instance = System.Activator.CreateInstance(type);
                foreach (var field in fields)
                {
                    if (field.IsStatic)
                        continue;

                    var val = new FieldValue(field, row , this);
                    field.SetValue(instance, val.Instance);
                }
                instances.Add(instance);                
            }

            return new Table(type,instances);
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

        IEnumerable<IRelatable> ITableable.FindRelatables(Type type)
        {
            Table table;
            if (_Tables.TryGetValue(type, out table))
            {
                return from row in table.Instances let relatable = row as IRelatable
                       where relatable != null 
                       select relatable;
            }
            return (new IRelatable[0]);
        }

        IEnumerable<T> ITableable.FindRows<T>()
        {            
            return Query<T>();
        }
    }
}
