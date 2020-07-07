using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public class RelationSorter
        {
            public readonly Type[] Types;
            public RelationSorter(IEnumerable<Type> table_queryables)
            {
                var tables = table_queryables.Select(q=> new RelationSorterTable(q) ).ToList();

                foreach(var table in tables)
                {
                    var type = table.Queryable;
                    var fields = type.GetFields();
                    foreach(var field in fields)
                    {
                        if (tables.Any(t => t.Queryable == field.FieldType))
                        {
                            table.Priority++;
                        }
                    }
                }

                tables.Sort(_Compare);
                Types = tables.Select(t => t.Queryable).ToArray();
            }

            private int _Compare(RelationSorterTable x, RelationSorterTable y)
            {
                return x.Priority - y.Priority;
            }
        }
    }
    

}
