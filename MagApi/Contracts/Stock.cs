using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class Stock
    {
        public string ComponentCode { get; set; }

        public string ComponentDescription { get; set; }

        public int ComponentQty{ get; set; }

        public IEnumerable<StockDetail> Details { get; set; }

    }
}
