using System;

namespace Regulus.RelationalTables
{
    namespace Raw
    {
        public struct Column
        {
            public readonly string Name;
            public readonly string Value;
            
            public Column(string name , string value)
            {
                Name = name;
                Value = value;
            }
        }
    }
}
