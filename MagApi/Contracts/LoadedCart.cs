using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class LoadedCart
    {
        public LoadedCart() { }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("areaid")]
        public long AreaId { get; set; }

        [JsonProperty("locationid")]
        public long LocationId { get; set; }

        [JsonProperty("cartid")]
        public long CartId { get; set; }

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("progressive")]
        public string Progressive { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("area")]
        public Area Area { get; set; }

        [JsonProperty("location")]
        public Location Location { get; set; }

        [JsonProperty("cart")]
        public Cart Cart { get; set; }

        [JsonProperty("datein")]
        public DateTime DateIn { get; set; }

        [JsonProperty("dateout")]
        public DateTime? DateOut { get; set; }

        [JsonProperty("loadedcartdetails")]
        public ICollection<LoadedCartDetail> LoadedCartDetails { get; set; }
    }
}
