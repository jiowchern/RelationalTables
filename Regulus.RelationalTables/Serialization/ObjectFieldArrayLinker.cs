using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Regulus.RelationalTables.Serialization.Binary
{
    internal class ObjectFieldArrayLinker : IObjectFieldLinkable
    {
        //private Stream _Stream;
        //private Binary.Field _Field;
        private FieldInfo _Info;
        private readonly object _Instance;
        private readonly IReadOnlyDictionary<int, object> _Instances;
        private readonly int _Length;
        readonly int[] _InstanceIds;
        public ObjectFieldArrayLinker(Stream stream, Field field, FieldInfo info, object instance, IReadOnlyDictionary<int, object> instances)
        {
            //this._Stream = stream;
            //this._Field = field;
            this._Info = info;
            this._Instance = instance;
            this._Instances = instances;

            stream.Position = field.Position;
            var length = (int)stream.ToValue();
            _Length = length;
            _InstanceIds = new int[length];
            for (int i = 0; i < length; i++)
            {
                var id = (int)stream.ToValue();
                _InstanceIds[i] = id;
            }
        }

        void IObjectFieldLinkable.Set()
        {
            
            var values = System.Array.CreateInstance(_Info.FieldType.GetElementType(), _Length);
            
            for (int i = 0; i < _Length; i++)
            {
               
                values.SetValue(_Instances[_InstanceIds[i]], i);
            }
            _Info.SetValue(_Instance , values);
        }
    }
}