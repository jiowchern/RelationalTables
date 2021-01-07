using System;
using System.Linq;

namespace Regulus.RelationalTables
{
    internal class TypeTreeNode
    {
        public readonly Type Type;

        readonly TypeTreeNode[] Childrens;
        
        public TypeTreeNode(Type type , TypeTreeNodeProviable type_tree_node_proviable)
        {
            Type = type;
            Childrens = _GetFields(type_tree_node_proviable);
            
        }

        private TypeTreeNode[] _GetFields(TypeTreeNodeProviable type_tree_node_proviable)
        {
            System.Collections.Generic.List<TypeTreeNode> nodes = new System.Collections.Generic.List<TypeTreeNode>();
            foreach (var field  in this.Type.GetFields())
            {
                Type type = null;
                if (field.FieldType.IsArray)
                    type = field.FieldType.GetElementType();
                else
                    type = field.FieldType;
                if (Type == type)
                    continue;
                var node = type_tree_node_proviable.Query(type);
                nodes.Add(node);
            }
            return nodes.ToArray();
        }

        internal int GetLevel()
        {
            if(Childrens.Length > 0)
                return Childrens.Max(owner => owner.GetLevel()) + 1;
            return 1;
        }
    }
}
