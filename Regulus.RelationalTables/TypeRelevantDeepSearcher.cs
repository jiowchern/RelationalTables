using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var types = from field in type.GetFields()
                        let fieldType = _GetFieldType(field)
                        where _Types.Any(t => type != fieldType && fieldType == t)
                        select fieldType;

            int maxLevel = level;

            foreach(var tableType in types)
            {
                maxLevel = _GetLevel(level + 1, tableType);
            }

            return maxLevel;
        }

        private Type _GetFieldType(FieldInfo field)
        {
            if (field.FieldType.HasElementType)
                return field.FieldType.GetElementType();
            return field.FieldType;
        }

        
    }
}
