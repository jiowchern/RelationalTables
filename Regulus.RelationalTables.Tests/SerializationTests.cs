using System.Linq;
namespace Regulus.RelationalTables.Tests
{
    namespace Serialization
    {
        public class FieldRepeatedReferenceTest_ItemB
        {
            public int Field;
        }
        public class FieldRepeatedReferenceTest_ItemA
        {
            public FieldRepeatedReferenceTest_ItemB Field1;
            public FieldRepeatedReferenceTest_ItemB Field2;
        }
        public class TableSerializTest_ItemC
        {
            public TableSerializTest_ItemA Field2;
            public float Field1;
        }
        public struct TableSerializTest_ItemB
        {
            public string Field1;
            public TableSerializTest_ItemC Field2;
        }
        public class TableSerializTest_ItemA
        {
            public int Field1;

            public TableSerializTest_ItemB[] Field2;

        }
        public class BinaryConverterTest_ItemD
        {
            public int Field;
            public BinaryConverterTest_ItemC ItemC;
        }
        public struct BinaryConverterTest_ItemC
        {
            public int Field;
            public BinaryConverterTest_ItemD FieldD;
        }
        public class BinaryConverterTest_ItemA
        {
            public int Field;
            public int[] FieldArray;
            
        }

        public class BinaryConverterTest_ItemB
        {
            public BinaryConverterTest_ItemA Field1;
            public BinaryConverterTest_ItemA[] Field2;

            public BinaryConverterTest_ItemB Field3;
        }

        public struct TypeSeparatorTest_ItemC
        {
            public float Field;
            public System.Guid Id;
            public TypeSeparatorTest_ItemB Field2;
            public object Field3;
        }
        public class TypeSeparatorTest_ItemB
        {
            public string Str;
            

        }
        public class TypeSeparatorTest_ItemA
        {
            
            public TypeSeparatorTest_ItemB Field1;
            public TypeSeparatorTest_ItemC Field2;
        }

        public class TypeSeparatorTest_ItemArrayA
        {
            public TypeSeparatorTest_ItemB[] Field1;
            public TypeSeparatorTest_ItemC[] Field2;
        }
    }
    public class SerializationTests
    {
        [NUnit.Framework.Test]
        public void BinaryConverterTestItemAField()
        {
            var outTables = BinaryConverterTest(new[] { new Table(typeof(Serialization.BinaryConverterTest_ItemA) , new[] { new Serialization.BinaryConverterTest_ItemA() {  Field = 1}  }) });
            var table =  outTables.Single();
            var itemA = table.Instances.Single() as Serialization.BinaryConverterTest_ItemA;
            NUnit.Framework.Assert.AreEqual(1 , itemA.Field);
        }
        [NUnit.Framework.Test]
        public void BinaryConverterTestItemFieldArray()
        {
            var outTables = BinaryConverterTest(new[] { new Table(typeof(Serialization.BinaryConverterTest_ItemA), new[] { new Serialization.BinaryConverterTest_ItemA() { FieldArray = new int[] { 1,2,3,4,5 } } }) });
            var table = outTables.Single();
            var itemA = table.Instances.Single() as Serialization.BinaryConverterTest_ItemA;
            NUnit.Framework.Assert.AreEqual(5, itemA.FieldArray[4]);
        }
        [NUnit.Framework.Test]
        public void BinaryConverterTestItemCD()
        {
            var itemC = new Serialization.BinaryConverterTest_ItemC();
            var itemD = new Serialization.BinaryConverterTest_ItemD();

            itemC.Field = 1;
            itemC.FieldD = itemD;

            itemD.Field = 2;
            itemD.ItemC = itemC;


            var outTables = BinaryConverterTest(new[] { new Table(typeof(Serialization.BinaryConverterTest_ItemD), new[] { itemD }) });
            var table = outTables.Single();
            var outItem = table.Instances.Single() as Serialization.BinaryConverterTest_ItemD;
            NUnit.Framework.Assert.AreEqual(1 , outItem.ItemC.Field );
            NUnit.Framework.Assert.AreEqual(2, outItem.ItemC.FieldD.Field);

        }
        [NUnit.Framework.Test]
        public void BinaryConverterTestItemB()
        {
            var itemB = new Serialization.BinaryConverterTest_ItemB();
            var itemA1 = new Serialization.BinaryConverterTest_ItemA();
            itemA1.Field = 1;
            itemA1.FieldArray = new[] { 1, 2, 3 };
            var itemA2 = new Serialization.BinaryConverterTest_ItemA();
            itemA2.Field = 2;
            itemA2.FieldArray = new[] { 2, 2, 3 };
            var itemA3 = new Serialization.BinaryConverterTest_ItemA();
            itemA3.Field = 3;
            itemA3.FieldArray = new[] { 3, 2, 3 };

            itemB.Field3 = itemB;
            itemB.Field1 = itemA1;
            itemB.Field2 = new[] { itemA2, itemA3 };
            var outTables = BinaryConverterTest(new[] { new Table(typeof(Serialization.BinaryConverterTest_ItemB), new[] { itemB }) });
            var table = outTables.Single();
            var outItemB = table.Instances.Single() as Serialization.BinaryConverterTest_ItemB;
            NUnit.Framework.Assert.AreEqual(outItemB, outItemB.Field3);
        }

        public System.Collections.Generic.IEnumerable<Table> BinaryConverterTest(System.Collections.Generic.IEnumerable<Table> tables)
        {
            var converter = new Regulus.RelationalTables.Serialization.BinaryConverter();
            var stream = new System.IO.MemoryStream();            
            converter.WriteToStream(tables, stream, new TypeProvider());
            stream.Position = 0;
            return converter.ReadFromStream(stream , new TypeProvider());
            
        }


        [NUnit.Framework.Test]
        public void TypeSeparatorTest()
        {
            var types = new RelationalTables.Serialization.NodeTypeSeparator(typeof(Serialization.TypeSeparatorTest_ItemA)).Types.ToArray();
            NUnit.Framework.Assert.AreEqual(typeof(Serialization.TypeSeparatorTest_ItemA), types[0]);
            NUnit.Framework.Assert.AreEqual(typeof(Serialization.TypeSeparatorTest_ItemB), types[1]);
            NUnit.Framework.Assert.AreEqual(typeof(Serialization.TypeSeparatorTest_ItemC), types[2]);

        }

        [NUnit.Framework.Test]
        public void TypeSeparatorArrayTest()
        {
            var types = new RelationalTables.Serialization.NodeTypeSeparator(typeof(Serialization.TypeSeparatorTest_ItemArrayA)).Types.ToArray();
            NUnit.Framework.Assert.AreEqual(typeof(Serialization.TypeSeparatorTest_ItemArrayA), types[0]);            
            NUnit.Framework.Assert.AreEqual(typeof(Serialization.TypeSeparatorTest_ItemB), types[1]);
            NUnit.Framework.Assert.AreEqual(typeof(Serialization.TypeSeparatorTest_ItemC), types[2]);

        }

        [NUnit.Framework.Test]
        public void FieldRepeatedReferenceTest()
        {
            var itema = new Serialization.FieldRepeatedReferenceTest_ItemA();
            var itemb = new Serialization.FieldRepeatedReferenceTest_ItemB();
            itemb.Field = 1;
            itema.Field1 = itemb;
            itema.Field2 = itemb;

            var converter = new Regulus.RelationalTables.Serialization.BinaryConverter();
            var stream = new System.IO.MemoryStream();
            converter.WriteToStream(new Table[] { 
                                    new Table(typeof(Serialization.FieldRepeatedReferenceTest_ItemA) , new []{itema ,itema}) 
                                    } , stream , new TypeProvider());
            stream.Position = 0;
            var tables = converter.ReadFromStream(stream, new TypeProvider()).ToArray();
            var outItema = tables.Where(t => t.Type == typeof(Serialization.FieldRepeatedReferenceTest_ItemA)).SelectMany(t => t.Instances).Single() as Serialization.FieldRepeatedReferenceTest_ItemA;

            NUnit.Framework.Assert.AreEqual(1 , outItema.Field1.Field);
            NUnit.Framework.Assert.AreEqual(1, outItema.Field2.Field);



        }
        [NUnit.Framework.Test]
        public void TableSerializTest()
        {

            var itemA = new Serialization.TableSerializTest_ItemA();
            itemA.Field1 = 1;

            var itemB = new Serialization.TableSerializTest_ItemB();
            itemB.Field1 = "test";
            itemA.Field2 = new[] { itemB };
            var itemC = new Serialization.TableSerializTest_ItemC();
            itemC.Field1 = 3f;
            itemC.Field2 = itemA;
            var stream = new System.IO.MemoryStream();

            var sourceTables = new Table[] { new Table(typeof(Serialization.TableSerializTest_ItemA) , new[] { itemA}) , 
                new Table(typeof(Serialization.TableSerializTest_ItemC) , new []{itemC } )  } ;
            var srcDb = new Database(sourceTables);

            var converter = new Regulus.RelationalTables.Serialization.BinaryConverter();
            converter.WriteToStream(srcDb.Tables, stream , new TypeProvider());
            stream.Position = 0;
            var outTables = converter.ReadFromStream(stream, new TypeProvider());

            var outDb = new Database(outTables);

            var resultItemA = outDb.Query<Serialization.TableSerializTest_ItemA>().Where(i => i.Field1 == 1).Single();
            var resultItemC = outDb.Query<Serialization.TableSerializTest_ItemC>().Where(i => i.Field2.Field1 == 1).Single();
            var resultItemB = outDb.Query<Serialization.TableSerializTest_ItemB>().Any();

            NUnit.Framework.Assert.AreEqual(1 , resultItemA.Field1);
            NUnit.Framework.Assert.AreEqual(1, resultItemC.Field2.Field1);
            NUnit.Framework.Assert.AreEqual("test", resultItemC.Field2.Field2[0].Field1);
            NUnit.Framework.Assert.AreEqual(false, resultItemB);
        }

    }
}
