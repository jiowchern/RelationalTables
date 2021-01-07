using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Regulus.RelationalTables
{
    public class TypeRelevantDeepSearcher
    {
        readonly TypeTree _Tree;
        public TypeRelevantDeepSearcher(params System.Type[] types) : this((System.Collections.Generic.IEnumerable<System.Type>)types)
        {
            
        }

        
        public TypeRelevantDeepSearcher(System.Collections.Generic.IEnumerable<System.Type> types) 
        {
            _Tree = new TypeTree(types);


        }
        public int Search(Type type)
        {
            return _Tree.GetNodeLevel(type);
        }

        
    }
}
