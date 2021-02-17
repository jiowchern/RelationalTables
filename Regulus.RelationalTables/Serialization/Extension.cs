using System;
using System.Linq;
using System.Collections.Generic;

namespace Regulus.RelationalTables.Serialization
{
    class IdProvider
    {
        readonly System.Collections.Generic.Dictionary<object, int> _Catch;
        int _Id;
        public IdProvider()
        {
            _Catch = new Dictionary<object, int>();
        }
        public int GetId(object instance)
        {
            int id;
            if(_Catch.TryGetValue(instance,out id))
            {
                return id;
            }
            id = ++_Id;
            _Catch.Add(instance , id);
            return id;
        }
    }
    
    public static class Extension
    {
        public static void ToDatabase(this System.IO.Stream stream , out Binary.Database database)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            database = (Binary.Database)formatter.Deserialize(stream);
        }
        public static object ToValue(this System.IO.Stream stream)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return formatter.Deserialize(stream);
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
