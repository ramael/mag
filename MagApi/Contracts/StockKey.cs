using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class StockKey
    {
        public string Code { get; set; }
        
        public string Description { get; set; }
    }

    public class StockKeyComparer : IEqualityComparer<StockKey>
    {
        public bool Equals(StockKey x, StockKey y)
        {
            return x.Code == y.Code;
        }

        public int GetHashCode([DisallowNull] StockKey obj)
        {
           return obj.Code.GetHashCode();
        }
    }
}
