namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public class RelationSorterTable
        {
            public int Priority;
            public readonly System.Type Queryable;

            public RelationSorterTable(System.Type queryable)
            {
                Queryable = queryable;
            }
        }
    }
    

}
