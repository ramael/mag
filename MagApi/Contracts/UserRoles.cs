using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class UserRoles
    {
        public UserRoles() { }

        [JsonProperty("roles")]
        public ICollection<string> Roles { get; set; }

    }
}
