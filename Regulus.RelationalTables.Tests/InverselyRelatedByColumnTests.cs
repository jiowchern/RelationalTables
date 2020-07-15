using NSubstitute;
using Regulus.RelationalTables.Raw;
using System.Linq;

namespace Regulus.RelationalTables.Tests
{
    public class InverselyRelatedByColumnTests
    {
        class Table1
        {            
            [Attributes.InverselyRelatedByColumn("Id")]
            public Table2[] Table2s;
        }
        class Table2 : IRelatable
        {
            public int Owner;
            public int Data;
            bool IRelatable.Compare(string val)
            {
                int owner;
                if (int.TryParse(val, out owner))
                {
                    return owner == Owner;
                }
                return false;
            }
        }

        readonly Column[] _Table1Row1 = new Column[] { new Column("Id", "1") };
        readonly Column[] _Table2Row1 = new Column[] { new Column(nameof(Table2.Owner), "1"), new Column(nameof(Table2.Data), "1") };
        readonly Column[] _Table2Row2 = new Column[] { new Column(nameof(Table2.Owner), "1"), new Column(nameof(Table2.Data), "2") };

        [NUnit.Framework.Test]
        public void Test()
        {
            var table1Row1 = NSubstitute.Substitute.For<Raw.IColumnProvidable>(); ;
            table1Row1.GetColumns().Returns(_Table1Row1);

            var table1 = NSubstitute.Substitute.For<Raw.IRowProvidable>();
            table1.GetRows().Returns(new Raw.IColumnProvidable[] { table1Row1 });
            table1.GetTableType().Returns(typeof(Table1));

            var table2Row1 = NSubstitute.Substitute.For<Raw.IColumnProvidable>(); ;
            table2Row1.GetColumns().Returns(_Table2Row1);

            var table2Row2 = NSubstitute.Substitute.For<Raw.IColumnProvidable>(); ;
            table2Row2.GetColumns().Returns(_Table2Row2);

            var table2 = NSubstitute.Substitute.For<Raw.IRowProvidable>();
            table2.GetRows().Returns(new Raw.IColumnProvidable[] { table2Row1, table2Row2 });
            table2.GetTableType().Returns(typeof(Table2));

            var db = new Regulus.RelationalTables.Database(new Raw.IRowProvidable[] { table1, table2 });
            var table1Config = db.Query<Table1>().First();
            NUnit.Framework.Assert.AreEqual(1, table1Config.Table2s[0].Data);
            NUnit.Framework.Assert.AreEqual(2, table1Config.Table2s[1].Data);

        }
    }
}
