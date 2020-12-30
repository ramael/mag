using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MagApi.Identity
{
    // Add profile data for application users by adding properties to the MagApplicationUser class
    public class MagApplicationUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(128)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(128)")]
        public string LastName { get; set; }
    }
}
