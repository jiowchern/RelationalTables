using Regulus.RelationalTables.Serialization;
using System;

namespace Regulus.RelationalTables.Tests
{
    internal class TypeProvider : ITypeProviable
    {
        object ITypeProviable.CreateInstance(string type_full_name)
        {
            var type= System.Type.GetType(type_full_name);
            return System.Activator.CreateInstance(type);
        }

        Type ITypeProviable.GetTypeByFullName(string type_full_name)
        {
            return System.Type.GetType(type_full_name); 
        }
    }
}