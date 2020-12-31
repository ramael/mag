using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class StockDetail
    {
        [JsonProperty("serialnumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("areaname")]
        public string AreaName { get; set; }

        [JsonProperty("locationname")]
        public string LocationName { get; set; }

    }
}
