using System;
using System.Linq;
using System.Collections.Generic;

namespace Regulus.RelationalTables.Serialization
{

    public static class Extension
    {
      

        public static IEnumerable<Table> ToTables(this Binary.Database db, ITypeProviable type_proviable)
        {
            var stream = new System.IO.MemoryStream(db.Buffer);
            var typeIndexByIdNumber = db.TypeIndexs.ToLookup(t => t.Id.Number);
            stream.Position = 0;
            var instances = db.CreateInstance(typeIndexByIdNumber, type_proviable, stream);

            foreach (var tableIndex in db.TableIndexs)
            {
                Binary.TypeIndex typeIndex = typeIndexByIdNumber[tableIndex.Type].Single();
                var type = type_proviable.GetTypeByFullName(typeIndex.Id.Name);
                yield return new Table(type, instances.Where(i => i.GetType().IsEquivalentTo(type)));
            }
        }
        
        public static object ToValue(this System.IO.Stream stream)
        {            
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return formatter.Deserialize(stream);
        }
        public static void ToDatabase(this System.IO.Stream stream, out Binary.Database database)
        {
            database = new Binary.Database();
            var reader = new System.IO.BinaryReader(stream);
            database.TableIndexs = _GetTableIndexs(reader).ToArray();
            database.TypeIndexs = _GetTypeIndexs(reader).ToArray();
            database.Objects = _GetObjects(reader).ToArray();

            database.Buffer = _GetBuffer(reader);
        }

        private static byte[] _GetBuffer(System.IO.BinaryReader reader)
        {
            var length = reader.ReadInt32();
            return reader.ReadBytes(length);
        }

        private static IEnumerable<Binary.Object> _GetObjects(System.IO.BinaryReader reader)
        {
            var length = reader.ReadInt32();

            for (int i = 0; i < length; i++)
            {
                var obj = new Binary.Object();
                obj.Id = reader.ReadInt32();
                obj.Type = reader.ReadInt32();
                obj.Fields = _GetObjectsFields(reader).ToArray();
                yield return obj;
            }
        }

        private static IEnumerable<Binary.Field> _GetObjectsFields(System.IO.BinaryReader reader)
        {
            var length = reader.ReadInt32();

            for (int i = 0; i < length; i++)
            {
                var field = new Binary.Field();
                field.Name = reader.ReadInt32();
                field.Position = reader.ReadInt64();
                yield return field;
            }
        }

        private static IEnumerable<Binary.TypeIndex> _GetTypeIndexs(System.IO.BinaryReader reader)
        {
            var length = reader.ReadInt32();
            
            for (int i = 0; i < length; i++)
            {
                var typeIndex = new Binary.TypeIndex();
                typeIndex.Id.Name = reader.ReadString();
                typeIndex.Id.Number = reader.ReadInt32();
                typeIndex.Fields = _GetTypeIndexsFields(reader).ToArray();
                yield return typeIndex;
            }
        }

        private static IEnumerable<Binary.Index> _GetTypeIndexsFields(System.IO.BinaryReader reader)
        {
            var length = reader.ReadInt32();

            for (int i = 0; i < length; i++)
            {
                yield return new Binary.Index() { Name = reader.ReadString(), Number = reader.ReadInt32() };
            }
                
        }

        private static IEnumerable<Binary.TableIndex> _GetTableIndexs(System.IO.BinaryReader reader)
        {
            var length = reader.ReadInt32();
            for (int i = 0; i < length; i++)
            {
                yield return new Binary.TableIndex() { Type = reader.ReadInt32() };
            }
        }

        public static void ToBinary(this ref Binary.Database database, System.IO.Stream stream)
        {
            var writer = new System.IO.BinaryWriter(stream);

            writer.Write(database.TableIndexs.Length);            
            foreach (var tableIndex in database.TableIndexs)
            {
                writer.Write(tableIndex.Type);                
            }

            writer.Write(database.TypeIndexs.Length);
            foreach (var typeIndex in database.TypeIndexs)
            {
                
                writer.Write(typeIndex.Id.Name);
                writer.Write(typeIndex.Id.Number);

                writer.Write(typeIndex.Fields.Length);
                foreach (var field in typeIndex.Fields)
                {
                    writer.Write(field.Name);
                    writer.Write(field.Number);
                }
            }
            writer.Write(database.Objects.Length);
            foreach (var obj in database.Objects)
            {
                writer.Write(obj.Id);
                writer.Write(obj.Type);
                writer.Write(obj.Fields.Length);
                foreach (var field in obj.Fields)
                {
                    writer.Write(field.Name);
                    writer.Write(field.Position);
                }
            }

            writer.Write(database.Buffer.Length);
            writer.Write(database.Buffer);


        }
        public static void ToBinary(this object instance,System.IO.Stream stream)
        {
            
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            formatter.Serialize(stream, instance);
        }
        public static bool IsRef(this System.Type type)
        {
            var code = System.Type.GetTypeCode(type);
            if (type.HasElementType && !type.GetElementType().IsRef())
                return false;

            if (code == TypeCode.Object)
            {                
                return true;
            }
                

            if (code != TypeCode.Empty && code != TypeCode.DBNull)
                return false;

            throw new Exception($"error type {type} , code {code}.");
        }
        static IdProvider _IdProvider = new IdProvider();
        public static int GetId(this object instance)
        {
            return _IdProvider.GetId(instance); 
        }
        public static int GetId(this System.Type type)
        {
            return _IdProvider.GetId(type);
        }

        public static int GetId(this System.Reflection.FieldInfo field)
        {
            return _IdProvider.GetId(field); 
        }

        public static Binary.Index ToIndex(this System.Reflection.FieldInfo info)
        {
            return new Binary.Index() { Number = info.GetId() , Name = info.Name };
        }
        public static Binary.TableIndex ToInfo(this Regulus.RelationalTables.Table table , IReadOnlyDictionary<string , Binary.TypeIndex> infos)
        {
            
            return new Binary.TableIndex() { Type = infos[table.Type.FullName].Id.Number };
            
        }
        public static bool TryGetIndex<TKey,TValue>(this ILookup<TKey, TValue> indexs , TKey id , ref TValue index)
        {
            
            if (indexs.Contains(id))
            {
                index = indexs[id].Single();
                return true;
            }
            return false;
        }
        
        public static Binary.TypeIndex ToIndex(this System.Type type)
        {
            var info = new Binary.TypeIndex(); 
            info.Id.Name = type.FullName;
            info.Id.Number = type.GetId();
            info.Fields = type.GetFields().Where(f => !f.IsStatic && f.IsPublic).Select(f => f.ToIndex()).ToArray();
            return info;
        }
    }
}
