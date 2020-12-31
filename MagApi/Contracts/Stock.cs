using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class Stock
    {
        [JsonProperty("componentcode")]
        public string ComponentCode { get; set; }

        [JsonProperty("componentdescription")]
        public string ComponentDescription { get; set; }

        [JsonProperty("componentqty")]
        public int ComponentQty{ get; set; }

        [JsonProperty("details")]
        public IEnumerable<StockDetail> Details { get; set; }

    }
}
