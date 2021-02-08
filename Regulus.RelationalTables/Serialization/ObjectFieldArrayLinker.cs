using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Regulus.RelationalTables.Serialization.Binary
{
    internal class ObjectFieldArrayLinker : IObjectFieldLinkable
    {
        private Stream _Stream;
        private Binary.Field _Field;
        private FieldInfo _Info;
        private readonly object _Instance;
        private readonly IDictionary<int, object> _Instances;

        public ObjectFieldArrayLinker(Stream stream, Field field, FieldInfo info, object instance, Dictionary<int, object> instances)
        {
            this._Stream = stream;
            this._Field = field;
            this._Info = info;
            this._Instance = instance;
            this._Instances = instances;
        }

        void IObjectFieldLinkable.Set()
        {
            _Stream.Position = _Field.Position;
            var length = (int)_Stream.ToValue();
            var values = System.Array.CreateInstance(_Info.FieldType.GetElementType(), length);
            
            for (int i = 0; i < length; i++)
            {
                var id = (int)_Stream.ToValue();
                values.SetValue(_Instances[id], i);
            }
            _Info.SetValue(_Instance , values);
        }
    }
}