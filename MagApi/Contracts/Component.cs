using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MagApi.Contracts
{
    public class Component
    {
        public Component() { }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }
    }
}
