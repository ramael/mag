using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MagApi.Identity
{
    public class MagApplicationRole: IdentityRole
    {
        [Column(TypeName = "nvarchar(256)")]
        public string Description { get; set; }
    }
}
