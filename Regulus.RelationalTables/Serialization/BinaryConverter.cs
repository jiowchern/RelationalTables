using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Regulus.RelationalTables.Serialization
{
    namespace Binary
    {
        [Serializable]
        public struct Field
        {
            public int Name;
            public long Position;

        }
        [Serializable]
        public struct Object
        {
            public int Type;
            public int Id;
            public Field[] Fields;
        }
        [Serializable]
        public struct Index
        {
            public string Name;
            public int Number;
        }
        [Serializable]
        public struct TypeIndex
        {
            public Index Id;
            public Index[] Fields;
        }
        [Serializable]
        public struct TableIndex
        {
            public int Type;
        }
        [Serializable]
        public struct Database
        {
            public TypeIndex[] TypeIndexs;
            public TableIndex[] TableIndexs;
            public Object[] Objects;
            public byte[] Buffer;

            internal IEnumerable<Table> ToTables(ITypeProviable type_proviable)
            {
                var stream = new System.IO.MemoryStream(Buffer);
                var typeIndexByIdNumber = TypeIndexs.ToLookup(t => t.Id.Number);
                stream.Position = 0;
                var instances = _CreateInstance(typeIndexByIdNumber, type_proviable, stream);

                foreach (var tableIndex in TableIndexs)
                {
                    TypeIndex typeIndex = typeIndexByIdNumber[tableIndex.Type].Single();
                    var type = type_proviable.GetTypeByFullName(typeIndex.Id.Name);
                    yield return new Table(type, instances.Where(i => i.GetType().IsEquivalentTo(type)));
                }
            }

            private IEnumerable<object> _CreateInstance(ILookup<int, TypeIndex> type_indices, ITypeProviable type_proviable, System.IO.Stream stream
                )
            {

                var instances = new System.Collections.Generic.Dictionary<int, object>();
                var linkers = new List<IObjectFieldLinkable>();
                foreach (var obj in Objects)
                {
                    var typeIndex = type_indices[obj.Type].Single();
                    var instance = type_proviable.CreateInstance(typeIndex.Id.Name);
                    instances.Add(obj.Id, instance);
                    var type = type_proviable.GetTypeByFullName(typeIndex.Id.Name);


                    foreach (var field in obj.Fields)
                    {
                        var fieldIndex = typeIndex.Fields.Single(f => f.Number == field.Name);
                        var info = type.GetField(fieldIndex.Name);
                        if (info.FieldType.IsRef())
                        {
                            if(info.FieldType.IsArray)
                            {
                                linkers.Add(new ObjectFieldArrayLinker(stream, field, info, instance, instances));
                            }
                            else
                                linkers.Add(new ObjectFieldLinker(stream, field, info, instance, instances));

                            continue;
                        }


                        stream.Position = field.Position;
                        info.SetValue(instance, stream.ToValue());
                    }

                }

                foreach (var linker in linkers)
                {
                    ((IObjectFieldLinkable)linker).Set();
                }

                return instances.Values;
            }
        }
    }
    
    public class BinaryConverter
    {        
        public BinaryConverter()
        {

        }
        public void WriteToStream(IEnumerable<Regulus.RelationalTables.Table> tables , System.IO.Stream stream , ITypeProviable type_proviable)
        {
            var database = new Binary.Database();
            var types =  new HashSet<System.Type>(tables.SelectMany(t => new NodeTypeSeparator(t.Type).Types));
            var typeIndexs = types.Select(t => t.ToIndex()).ToArray();           
            database.TypeIndexs = typeIndexs;
            database.TableIndexs = _CreateTableIndexs(tables.Select(t=>t.Type)).ToArray();           
            var buffer= new System.IO.MemoryStream();
            database.Objects = _CreateObjectFields(_CreateObjects(tables),  typeIndexs, buffer, type_proviable).ToArray();
            buffer.Position = 0;
            database.Buffer = buffer.GetBuffer();
            database.ToBinary(stream);
        }

        private IEnumerable<Binary.Object> _CreateObjectFields(IEnumerable<Tuple<Binary.Object, object>> objects, IEnumerable<Binary.TypeIndex> type_indices ,System.IO.Stream stream , ITypeProviable type_proviable)
        {
            
            
            var typeIndexsByIdNumber = type_indices.ToLookup(t => t.Id.Number);
            
            foreach (var objTuple in objects)
            {
                var obj = objTuple.Item1;

                var instance = objTuple.Item2;
                var typeIndex = typeIndexsByIdNumber[obj.Type].Single();
                
                var fields = new List<Binary.Field>();
                foreach (var fieldIndex in typeIndex.Fields)
                {
                    
                    var type = type_proviable.GetTypeByFullName(typeIndex.Id.Name);
                    var fieldInfo = type.GetField(fieldIndex.Name);
                    var fieldInstance = fieldInfo.GetValue(instance);
                    if (fieldInstance == null)
                        continue;


                    var field = new Binary.Field();
                    field.Name = fieldIndex.Number;
                    field.Position = stream.Position;
                    fields.Add(field);

                    if (fieldInfo.FieldType.IsRef())
                    {
                        if (fieldInfo.FieldType.IsArray)
                        {
                            _CreateRefFieldArrayValue(instance, fieldInfo, stream);
                        }
                        else
                            _CreateRefFieldValue(instance, fieldInfo, stream);
                    }
                    else
                    {
                        _CreateFieldValue(instance, fieldInfo, stream);
                    }
                }
                obj.Fields = fields.ToArray();
                yield return obj;
            }


        }

        private void _CreateRefFieldArrayValue(object instance, System.Reflection.FieldInfo info, Stream stream)
        {
            var fieldInstance = info.GetValue(instance);
            var fieldItems = fieldInstance as System.Collections.IList;
            fieldItems.Count.ToBinary(stream);
            for (int i = 0; i < fieldItems.Count; i++)
            {
                fieldItems[i].GetId().ToBinary(stream);
            }                        
        }

        private void _CreateFieldValue(object instance, System.Reflection.FieldInfo info, Stream stream)
        {
            var fieldInstance = info.GetValue(instance);
            fieldInstance.ToBinary(stream);
        }

        private void _CreateRefFieldValue(object instance, System.Reflection.FieldInfo info,System.IO.Stream stream)
        {
            var fieldInstance = info.GetValue(instance);
            fieldInstance.GetId().ToBinary(stream);
        }

        private IEnumerable<Tuple<Binary.Object,object> > _CreateObjects(IEnumerable<Table> tables )
        {

            var exist = new HashSet<object>();
            foreach (var table in tables)
            {
                
                foreach (var instance in table.Instances)
                {
                    foreach (var obj in _CreateObject(table.Type, instance, exist))
                    {
                        yield return obj;
                    }
                }
            }
        }

        private IEnumerable<Tuple<Binary.Object, object>> _CreateObject(Type type, object instance, HashSet<object> exist)
        {
            if (exist.Contains(instance))
                yield break;

            exist.Add(instance);
            var obj = new Binary.Object();
            obj.Id = instance.GetId();
            obj.Fields = default;
            obj.Type = type.GetId();
            yield return new Tuple<Binary.Object, object>(obj , instance);

            foreach (var fieldInfo in type.GetFields())
            {
                if (fieldInfo.IsStatic)
                    continue;
                if (fieldInfo.IsPrivate)
                    continue;
                if(fieldInfo.FieldType.IsRef())
                {
                    if(fieldInfo.FieldType.IsArray)
                    {
                        var values = fieldInfo.GetValue(instance) as System.Collections.IList;
                        for (int i = 0; i < values.Count; i++)
                        {
                            foreach (var fieldObj in _CreateObject(fieldInfo.FieldType.GetElementType(), values[i], exist))
                            {
                                yield return fieldObj;
                            }
                        }
                    }
                    else
                    {
                        foreach (var fieldObj in _CreateObject(fieldInfo.FieldType, fieldInfo.GetValue(instance), exist))
                        {
                            yield return fieldObj;
                        }


                    }
                }

            }
            
        }

        private IEnumerable<Binary.TableIndex> _CreateTableIndexs(IEnumerable<System.Type> types)
        {
            foreach (var type in types)
            {
                var id = type.GetId();
                yield return new Binary.TableIndex() { Type = id };
            }
        }

        public IEnumerable<Regulus.RelationalTables.Table> ReadFromStream(System.IO.Stream stream , ITypeProviable type_proviable)
        {
            Binary.Database database;
            stream.ToDatabase(out database);
            return database.ToTables(type_proviable);
        }
    }
}

