using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Regulus.RelationalTables.Serialization
{
    interface IObjectFieldLinkable
    {
        void Set();
    }
    internal class ObjectFieldLinker : IObjectFieldLinkable
    {
        private Stream _Stream;
        private Binary.Field _Field;
        private FieldInfo _Info;
        private readonly object _Instance;
        private readonly IDictionary<int, object> _Instances;

        public ObjectFieldLinker(Stream stream, Binary.Field field, FieldInfo info, object instance  , System.Collections.Generic.IDictionary<int , object> instances)
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
            var id = (int)_Stream.ToValue();
            
            _Info.SetValue(_Instance, _Instances[id]);            
        }
    }
}