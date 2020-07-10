using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Regulus.RelationalTables
{

    public class TypeRelevantDeepSearcher
    {
        readonly System.Type[] _Types;
        public TypeRelevantDeepSearcher(params System.Type[] types)
        {
            _Types = types;
        }
        public TypeRelevantDeepSearcher(System.Collections.Generic.IEnumerable<System.Type> types) : this(types.ToArray())
        {

        }
        public int Search(Type type)
        {
            
            return _GetLevel(0 , type);
        }

        private int _GetLevel(int level, Type type)
        {
            var fields = from field in type.GetFields() where _Types.Any(t => t == field.FieldType) select field;

            var maxLevel = level;
            foreach (var f in fields)
            {
                var l = _GetLevel(level+1 , f.FieldType);
                if(maxLevel < l)
                {
                    maxLevel = l;
                }
            }

            return maxLevel;
        }
    }
}
