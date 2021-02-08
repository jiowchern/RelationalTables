using System.Collections.Generic;

namespace Regulus.RelationalTables.Serialization
{
    public class NodeTypeSeparator
    {
        public readonly IReadOnlyCollection<System.Type> Types;
        
        
        public NodeTypeSeparator(System.Type type)
        {
            
            var set = new HashSet<System.Type>();
            Types = set;
            _Separat(type, set);
        }

        private void _Separat(System.Type type , HashSet<System.Type> set)
        {
            if (type.IsArray)
                type = type.GetElementType();
            if (set.Contains(type))
                return;

            var code = System.Type.GetTypeCode(type);
            if (code != System.TypeCode.Object)
            {
                return;
            }

            set.Add(type);

            foreach (var field in type.GetFields())
            {
                if (field.IsStatic == true)
                    continue;

                if (field.IsPrivate == true)
                    continue;

                _Separat(field.FieldType, set);
            }

            
        }
        
    }
}
