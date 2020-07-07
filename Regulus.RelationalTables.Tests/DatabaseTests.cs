using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Regulus.RelationalTables.Raw;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables.Tests
{
    public class TestConfig1 : Regulus.RelationalTables.IRelatable
    {
        
        public int Field1;
        public string Field2;
        public float Field3;

        bool IRelatable.Compare(string val)
        {
            int outVal;
            if (int.TryParse(val, out outVal))
            {
                return outVal == Field1;
            }

            return false;
        }
    }

    public class TestConfig2
    {
        [Regulus.RelationalTables.Array("Field10", "Field11", "Field12")]   
        public int[] Field1;
    }

    public class TestConfig3
    {        
        public TestConfig1 Field1;
    }

    public class TestConfig4
    {
        public enum ENUM { A,B,C}
        public ENUM Field1;
    }



    public class DatabaseTests
    {
        

        [Test]
        public void DatabaseQueryTest()
        {
            
            var config1Row1 = NSubstitute.Substitute.For<IColumnProvidable>(); ;
            config1Row1.GetColumns().Returns(_ReturnColumn1);

            var config1 = NSubstitute.Substitute.For<IRowProvidable>();
            config1.GetRows().Returns( new IColumnProvidable[] { config1Row1 } );
            config1.GetTableType().Returns( typeof(TestConfig1));

            var db = new Regulus.RelationalTables.Database(new IRowProvidable[] { config1 });
            var config = db.Query<TestConfig1>().First();
            Assert.AreEqual(1 , config.Field1 );
            Assert.AreEqual("2", config.Field2);
            Assert.AreEqual(3f, config.Field3);
        }

        [Test]
        public void DatabaseQueryRelationTest()
        {

            var config1Row1 = NSubstitute.Substitute.For<IColumnProvidable>(); ;
            config1Row1.GetColumns().Returns(_ReturnColumn1);

            var config1 = NSubstitute.Substitute.For<IRowProvidable>();
            config1.GetRows().Returns(new IColumnProvidable[] { config1Row1 });
            config1.GetTableType().Returns(typeof(TestConfig1));


            var config1Row3 = NSubstitute.Substitute.For<IColumnProvidable>(); ;
            config1Row3.GetColumns().Returns(_ReturnColumn3);

            var config3 = NSubstitute.Substitute.For<IRowProvidable>();
            config3.GetRows().Returns(new IColumnProvidable[] { config1Row3 });
            config3.GetTableType().Returns(typeof(TestConfig3));

            var db = new Regulus.RelationalTables.Database(new IRowProvidable[] { config3 , config1 });
            var config = db.Query<TestConfig3>().First();
            Assert.AreEqual(1, config.Field1.Field1);
            Assert.AreEqual("2", config.Field1.Field2);
            Assert.AreEqual(3f, config.Field1.Field3);
        }

        [Test]
        public void FieldValueTest()
        {
            var table = NSubstitute.Substitute.For<ITableFindable>();
            var type = typeof(TestConfig1);
            var field = type.GetField(nameof(TestConfig1.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns( _ReturnColumn1 );
            var val = new Regulus.RelationalTables.FieldValue(field , row , table);

            Assert.AreEqual(1 , val.Instance);
        }
        [Test]
        public void FieldValueEnumTest()
        {
            var table = NSubstitute.Substitute.For<ITableFindable>();
            var type = typeof(TestConfig4);
            var field = type.GetField(nameof(TestConfig4.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(_ReturnColumn4);
            var val = new Regulus.RelationalTables.FieldValue(field, row, table);

            Assert.AreEqual(TestConfig4.ENUM.A, val.Instance);
        }

            [Test]
        public void FieldValueArrayTest()
        {

            var type = typeof(TestConfig2);
            var field = type.GetField(nameof(TestConfig2.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(_ReturnColumn2);
            var val = new Regulus.RelationalTables.FieldValue(field, row, null);
            var values = val.Instance as int[];
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void FieldValueRelationTest()
        {
            var table = NSubstitute.Substitute.For<ITableFindable>();
            table.FindRows(NSubstitute.Arg.Is(typeof(TestConfig1))).Returns(_ReturnTestConfig1 );
            var type = typeof(TestConfig3);
            var field = type.GetField(nameof(TestConfig3.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(_ReturnColumn3);
            var val = new Regulus.RelationalTables.FieldValue(field, row, table);
            var config1 = val.Instance as TestConfig1;
            Assert.AreEqual(1, config1.Field1);
            Assert.AreEqual("2", config1.Field2);
            Assert.AreEqual(3f, config1.Field3);
        }

        private IEnumerable<object> _ReturnTestConfig1(CallInfo arg)
        {
            return new object[] { new TestConfig1() { Field1 = 1, Field2 = "2", Field3 = 3f } };
        }

        private IEnumerable<Column> _ReturnColumn3(CallInfo arg)
        {
            return new Column[] { new Column("Field1", "1")};
        }

        private IEnumerable<Column> _ReturnColumn4(CallInfo arg)
        {
            return new Column[] { new Column("Field1", "A") };
        }

        private IEnumerable<Column> _ReturnColumn2(CallInfo arg)
        {
            return new Column[] { new Column("Field10", "1"), new Column("Field11", "2"), new Column("Field12", "3") };
        }

        private IEnumerable<Column> _ReturnColumn1(CallInfo arg)
        {
            return new Column[] { new Column(nameof(TestConfig1.Field1) , "1") , new Column(nameof(TestConfig1.Field2), "2") , new Column(nameof(TestConfig1.Field3), "3") };
        }


        [Test]
        public void TypeSortTest()
        {
            var sorter = new RelationSorter(new Type[] { typeof(TestConfig3), typeof(TestConfig1)});
            

            Assert.AreEqual(typeof(TestConfig1)  , sorter.Types[0]);
            Assert.AreEqual(typeof(TestConfig3), sorter.Types[1]);

        }
}
}
