using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Regulus.RelationalTables.Raw;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Regulus.RelationalTables.Tests
{
    public class TestConfig1
    {
        public int Field1;
        public string Field2;
        public float Field3;
    }

    public class TestConfig2
    {
        [Regulus.RelationalTables.Array("Field10", "Field11", "Field12")]   
        public int[] Field1;
    }



    public class DatabaseTests
    {
        

        [Test]
        public void DatabaseQueryTest()
        {
            
            var config1Row1 = NSubstitute.Substitute.For<IRowQueryable>(); ;
            config1Row1.GetColumns().Returns(_ReturnColumn1);

            var config1 = NSubstitute.Substitute.For<ITableQueryable>();
            config1.GetRows().Returns( new IRowQueryable[] { config1Row1 } );
            config1.GetType().Returns( typeof(TestConfig1));

            var db = new Regulus.RelationalTables.Database(new ITableQueryable[] { config1 });
            var config = db.Query<TestConfig1>().First();
            Assert.AreEqual(1 , config.Field1 );
            Assert.AreEqual("2", config.Field2);
            Assert.AreEqual(3f, config.Field3);
        }

        [Test]
        public void FieldValueTest()
        {

            var type = typeof(TestConfig1);
            var field = type.GetField(nameof(TestConfig1.Field1));
            var row = NSubstitute.Substitute.For<IRowQueryable>();
            row.GetColumns().Returns( _ReturnColumn1 );
            var val = new Regulus.RelationalTables.FieldValue(field , row , null);

            Assert.AreEqual(1 , val.Instance);
        }

        [Test]
        public void FieldValueArrayTest()
        {

            var type = typeof(TestConfig2);
            var field = type.GetField(nameof(TestConfig2.Field1));
            var row = NSubstitute.Substitute.For<IRowQueryable>();
            row.GetColumns().Returns(_ReturnColumn2);
            var val = new Regulus.RelationalTables.FieldValue(field, row, null);
            var values = val.Instance as int[];
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        private IEnumerable<Column> _ReturnColumn2(CallInfo arg)
        {
            return new Column[] { new Column("Field10", "1"), new Column("Field11", "2"), new Column("Field12", "3") };
        }

        private IEnumerable<Column> _ReturnColumn1(CallInfo arg)
        {
            return new Column[] { new Column(nameof(TestConfig1.Field1) , "1") , new Column(nameof(TestConfig1.Field2), "2") , new Column(nameof(TestConfig1.Field3), "3") };
        }
    }
}
