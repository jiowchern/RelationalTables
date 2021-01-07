using NUnit.Framework;
using System;

namespace Regulus.RelationalTables.Tests
{
    public class TypeLevelTests
    {
        public class TestSortConfig1
        {
        }

        public class TestSortConfig2
        {

            public TestSortConfig1 Field1;
            public TestSortConfig1 Field2;
            public TestSortConfig2 Field3;
        }

        public class TestSortConfig3
        {
            public TestSortConfig2 Field1;
        }

        public class TestSortConfig4Inner
        {
            public TestSortConfig3 Field1;
        }
        public class TestSortConfig4
        {
            public TestSortConfig4Inner Field1;
        }

        public class TestSortArray1
        {
            public TestSortArray2[] Field1;
        }

        public class TestSortArray2
        {
            public int Field1;
        }

        [NUnit.Framework.Test]
        public void TypeLevelArrayField()
        {
            var level = new TypeRelevantDeepSearcher(new Type[] { typeof(TestSortArray1), typeof(TestSortArray2)});


            Assert.AreEqual(3, level.Search(typeof(TestSortArray1)));
            Assert.AreEqual(2, level.Search(typeof(TestSortArray2)));


        }

        [NUnit.Framework.Test]
        public void TypeLevel1()
        {
            var level = new TypeRelevantDeepSearcher(new Type[] { typeof(TestSortConfig1), typeof(TestSortConfig2), typeof(TestSortConfig3) });


            Assert.AreEqual(1, level.Search(typeof(TestSortConfig1)));
            Assert.AreEqual(2, level.Search(typeof(TestSortConfig2)));
            Assert.AreEqual(3, level.Search(typeof(TestSortConfig3)));

        }

        [Test]
        public void TypeSortTest2()
        {
            var sorter = new Raw.RelationSorter(new Type[] { typeof(TestSortConfig1), typeof(TestSortConfig2), typeof(TestSortConfig3) });


            Assert.AreEqual(typeof(TestSortConfig1), sorter.Types[0]);
            Assert.AreEqual(typeof(TestSortConfig2), sorter.Types[1]);
            Assert.AreEqual(typeof(TestSortConfig3), sorter.Types[2]);

        }
       
    }
}
