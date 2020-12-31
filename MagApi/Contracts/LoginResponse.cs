using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MagApi.Contracts
{
    public class LoginResponse
    {
        public LoginResponse() { }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("roles")]
        public IList<string> Roles { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

    }
}
