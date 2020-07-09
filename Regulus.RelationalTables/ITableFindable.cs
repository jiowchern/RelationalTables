using System;
using System.Collections.Generic;

namespace Regulus.RelationalTables
{
    public interface ITableable
    {
        IEnumerable<IRelatable> FindRelatables(Type type);
        IEnumerable<T> FindRows<T>();

    }
}