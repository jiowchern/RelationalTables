using NSubstitute;
using NUnit.Framework;
using Regulus.RelationalTables.Raw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
        [Regulus.RelationalTables.Attributes.Merge("Field10", "Field11", "Field12")]   
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
    public class CustomFieldParser : Regulus.RelationalTables.Attributes.FieldParser
    {
        public override object Parse(FieldInfo field, IEnumerable<Column> row, ITableable table)
        {
            var col = row.Single(c => c.Name == nameof(TestConfig5.Field1));
            if (col.Value == "AAA")
                return 1;
            return 0;
        }
    }
    public class TestConfig5
    {
        [CustomFieldParser()]
        public int Field1;        
    }
    public class DatabaseTests
    {

        [Test]
        public void DatabaseQueryTest()
        {
            
            var config1Row1 = NSubstitute.Substitute.For<IColumnProvidable>(); ;
            config1Row1.GetColumns().Returns(DataProvider._ReturnColumn1);

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
            config1Row1.GetColumns().Returns(DataProvider._ReturnColumn1);

            var config1 = NSubstitute.Substitute.For<IRowProvidable>();
            config1.GetRows().Returns(new IColumnProvidable[] { config1Row1 });
            config1.GetTableType().Returns(typeof(TestConfig1));


            var config1Row3 = NSubstitute.Substitute.For<IColumnProvidable>(); ;
            config1Row3.GetColumns().Returns(DataProvider._ReturnColumn3);

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
        public void TypeSortTest()
        {
            var sorter = new RelationSorter(new Type[] { typeof(TestConfig3), typeof(TestConfig1)});
            

            Assert.AreEqual(typeof(TestConfig1)  , sorter.Types[0]);
            Assert.AreEqual(typeof(TestConfig3), sorter.Types[1]);

        }


    }
}
