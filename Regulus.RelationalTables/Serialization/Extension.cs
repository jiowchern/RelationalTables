using System;
using System.Linq;
using System.Collections.Generic;

namespace Regulus.RelationalTables.Serialization
{
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
        public static int GetId(this object instance)
        {
            return instance.GetHashCode();
        }
        public static int GetId(this System.Type type)
        {
            return type.GetHashCode();
        }

        public static int GetId(this System.Reflection.FieldInfo field)
        {
            return field.GetHashCode();
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
