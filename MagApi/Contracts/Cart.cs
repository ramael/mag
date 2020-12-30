using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class Cart
    {

        public enum StatusEnum 
        { 
            Available = 0,
            NotAvailable = 1,
            UnderRepair = 2,
            Destroyed = 3
        }

        public Cart() { }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("serialnumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("status")]
        public StatusEnum Status { get; set; }
    }
}
