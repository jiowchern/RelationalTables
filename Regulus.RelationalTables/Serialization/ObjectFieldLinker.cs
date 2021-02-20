using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Regulus.RelationalTables.Serialization
{
   
    internal class ObjectFieldLinker : IObjectFieldLinkable
    {
        
        private FieldInfo _Info;
        private readonly object _Instance;
        private readonly IDictionary<int, object> _Instances;

        readonly int _InstanceId;

        public ObjectFieldLinker(Stream stream, Binary.Field field, FieldInfo info, object instance  , System.Collections.Generic.IDictionary<int , object> instances)
        {
            stream.Position = field.Position;
            var id = (int)stream.ToValue();
            _InstanceId = id;
            this._Info = info;
            this._Instance = instance;
            this._Instances = instances;
        }

        void IObjectFieldLinkable.Set()
        {
            
            
            _Info.SetValue(_Instance, _Instances[_InstanceId]);            
        }
    }
}