using System.Collections.Generic;

namespace MagApi.Identity.Helpers
{
    public interface ITokenHelper
    {
        string GenerateToken(string username, IEnumerable<string> roles);
    }

}
