using NSubstitute.Core;
using Regulus.RelationalTables.Raw;
using System.Collections.Generic;

namespace Regulus.RelationalTables.Tests
{
    public class DataProvider
    {
        public static IEnumerable<Column> _ReturnColumn1(CallInfo arg)
        {
            return new Column[] { new Column(nameof(TestConfig1.Field1), "1"), new Column(nameof(TestConfig1.Field2), "2"), new Column(nameof(TestConfig1.Field3), "3") };
        }

        public static IEnumerable<IRelatable> _ReturnTestConfig1(CallInfo arg)
        {
            return new IRelatable[] { new TestConfig1() { Field1 = 1, Field2 = "2", Field3 = 3f } };
        }

        public static IEnumerable<Column> _ReturnColumn3(CallInfo arg)
        {
            return new Column[] { new Column("Field1", "1") };
        }

        public static IEnumerable<Column> _ReturnColumn4(CallInfo arg)
        {
            return new Column[] { new Column("Field1", "A") };
        }

        public static IEnumerable<Column> _ReturnColumn5(CallInfo arg)
        {
            return new Column[] { new Column("Field1", "AAA") };
        }

        public static IEnumerable<Column> _ReturnColumn2(CallInfo arg)
        {
            return new Column[] { new Column("Field10", "1"), new Column("Field11", "2"), new Column("Field12", "3") };
        }



    }
}
