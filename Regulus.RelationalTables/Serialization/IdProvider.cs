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
}
