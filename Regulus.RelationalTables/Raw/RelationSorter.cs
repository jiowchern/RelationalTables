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
                var typeLevel = new TypeRelevantDeepSearcher(tables.Select(t=>t.Type));
                foreach (var table in tables)
                {
                    table.Priority = typeLevel.Search(table.Type);
                }

                tables.Sort(_Compare);
                Types = tables.Select(t => t.Type).ToArray();
            }

            

            private int _Compare(RelationSorterTable x, RelationSorterTable y)
            {
                return x.Priority - y.Priority;
            }
        }
    }
    

}
