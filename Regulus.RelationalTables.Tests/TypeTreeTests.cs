
namespace Regulus.RelationalTables.Tests
{

    namespace Circulation
    {
    }
    public class TypeTreeTests
    {
        public class Type2
        {
            
        }
        public class Type1
        {
            public Type2[] Field1;
            public Type2[] Field2;
        }
        public class Type0
        {
            public Type1[] Field1;
        }
        public class TypeNull
        {

        }

        public class TypeCycle
        {
            public TypeCycle Field1;
        }


        [NUnit.Framework.Test]
        public void TypeNullGetNodeLevelTest()
        {
            var tree = new TypeTree(new[] { typeof(Type0) });

            var ret = tree.GetNodeLevel(typeof(TypeNull));

            NUnit.Framework.Assert.AreEqual(0, ret);
        }

        [NUnit.Framework.Test]
        public void Type1GetNodeLevelTest()
        {
            var tree = new TypeTree(new[] { typeof(Type0) });

            var ret0 = tree.GetNodeLevel(typeof(Type0));
            var ret1 = tree.GetNodeLevel(typeof(Type1));
            var ret2 = tree.GetNodeLevel(typeof(Type2)); 
            


            NUnit.Framework.Assert.AreEqual(3, ret0);
            NUnit.Framework.Assert.AreEqual(2, ret1);
            NUnit.Framework.Assert.AreEqual(1, ret2);
        }

        [NUnit.Framework.Test]
        public void TypeCycleGetNodeLevelTest()
        {
            var tree = new TypeTree(new[] { typeof(TypeCycle) });

            var ret0 = tree.GetNodeLevel(typeof(TypeCycle));

            NUnit.Framework.Assert.AreEqual(1, ret0);
            
        }


        [NUnit.Framework.Test]
        public void TypeCycleAndType0GetNodeLevelTest()
        {
            var tree = new TypeTree(new[] { typeof(TypeCycle) , typeof(Type0) });

            var ret0 = tree.GetNodeLevel(typeof(TypeCycle));
            var ret1 = tree.GetNodeLevel(typeof(Type0));

            NUnit.Framework.Assert.AreEqual(1, ret0);
            NUnit.Framework.Assert.AreEqual(3, ret1);

        }
    }
}
