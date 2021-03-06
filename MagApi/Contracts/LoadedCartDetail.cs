﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class LoadedCartDetail
    {
        public enum StatusEnum { 
            Original = 0,
            New = 1,
            Modified = 2,
            Deleted = 3
        }

        public LoadedCartDetail() { }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("loadedcartid")]
        public long LoadedCartId { get; set; }

        [JsonProperty("componentid")]
        public long ComponentId { get; set; }

        [JsonProperty("component")]
        public Component Component { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("status")]
        public StatusEnum Status { get; set; }
    }
}
