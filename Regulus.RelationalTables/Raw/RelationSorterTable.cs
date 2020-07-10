namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public class RelationSorterTable
        {
            public int Priority;
            public readonly System.Type Type;

            public RelationSorterTable(System.Type Type)
            {
                this.Type = Type;
            }
        }
    }
    

}
