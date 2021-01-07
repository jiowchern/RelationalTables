using System;
using System.Linq;

namespace Regulus.RelationalTables
{
    public class TypeTree : TypeTreeNodeProviable
    {

        readonly System.Collections.Generic.Dictionary<Type, TypeTreeNode> _Nodes;
        readonly TypeTreeNodeProviable _Provider;
        public TypeTree(System.Collections.Generic.IEnumerable<System.Type> types)
        {
            _Nodes = new System.Collections.Generic.Dictionary<Type, TypeTreeNode>();
            _Provider = this;

            
            foreach (var type in types)
            {            
                var node = _Provider.Query(type);
                if(!_Nodes.ContainsKey(type))
                {
                    _Nodes.Add(type , node);
                }
            }

        }
        TypeTreeNode _QueryNode(Type type)
        {
            TypeTreeNode node;
            if (!_Nodes.TryGetValue(type , out node))
            {
                node = new TypeTreeNode(type , this);
                _Nodes.Add(type , node);
            }
            return node;
        }
        public int GetNodeLevel(Type type)
        {
            TypeTreeNode node;
            if (!_Nodes.TryGetValue(type, out node))
                return 0;

            return node.GetLevel();
        }

        TypeTreeNode TypeTreeNodeProviable.Query(Type type)
        {
            return _QueryNode(type);
        }
    }
}
