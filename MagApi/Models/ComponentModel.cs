using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    [Table("components")]
    public class ComponentModel : BaseModel
    {
        [Required(AllowEmptyStrings = false)]
        [IndexColumn("IDX_components_code", IsUnique = true)]
        [MaxLength(16)]
        public string Code { get; set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(256)]
        public string Description { get; set; }

        public string Notes { get; set; }

    }
}
