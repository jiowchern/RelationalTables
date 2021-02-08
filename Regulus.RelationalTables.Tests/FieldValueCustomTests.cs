using NSubstitute;
using NUnit.Framework;
using Regulus.RelationalTables.Raw;

namespace Regulus.RelationalTables.Tests
{
    public class FieldValueTests
    {
        [Test]
        public void FieldValueTest()
        {
            var table = NSubstitute.Substitute.For<ITableable>();
            var type = typeof(TestConfig1);
            var field = type.GetField(nameof(TestConfig1.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(DataProvider._ReturnColumn1);
            var val = new Regulus.RelationalTables.FieldValue(field, row, table);

            Assert.AreEqual(1, val.Instance);
        }

        [Test]
        public void FieldValueCustomTest()
        {
            var table = NSubstitute.Substitute.For<ITableable>();
            var type = typeof(TestConfig5);
            var field = type.GetField(nameof(TestConfig5.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(DataProvider._ReturnColumn5);
            var val = new Regulus.RelationalTables.FieldValue(field, row, table);

            Assert.AreEqual(1, val.Instance);
        }

        [Test]
        public void FieldValueEnumTest()
        {
            var table = NSubstitute.Substitute.For<ITableable>();
            var type = typeof(TestConfig4);
            var field = type.GetField(nameof(TestConfig4.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(DataProvider._ReturnColumn4);
            var val = new Regulus.RelationalTables.FieldValue(field, row, table);

            Assert.AreEqual(TestConfig4.ENUM.A, val.Instance);
        }

        [Test]
        public void FieldValueArrayTest()
        {

            var type = typeof(TestConfig2);
            var field = type.GetField(nameof(TestConfig2.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(DataProvider._ReturnColumn2);
            var val = new Regulus.RelationalTables.FieldValue(field, row, null);
            var values = val.Instance as int[];
            Assert.AreEqual(1, values[0]);
            Assert.AreEqual(2, values[1]);
            Assert.AreEqual(3, values[2]);
        }

        [Test]
        public void FieldValueRelationTest()
        {
            var table = NSubstitute.Substitute.For<ITableable>();
            table.FindRelatables(NSubstitute.Arg.Is(typeof(TestConfig1))).Returns(DataProvider._ReturnTestConfig1);
            var type = typeof(TestConfig3);
            var field = type.GetField(nameof(TestConfig3.Field1));
            var row = NSubstitute.Substitute.For<IColumnProvidable>();
            row.GetColumns().Returns(DataProvider._ReturnColumn3);
            var val = new Regulus.RelationalTables.FieldValue(field, row, table);
            var config1 = (TestConfig1)val.Instance  ;
            Assert.AreEqual(1, config1.Field1);
            Assert.AreEqual("2", config1.Field2);
            Assert.AreEqual(3f, config1.Field3);
        }

    }
}
