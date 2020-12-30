using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    [Table("warehouses")]
    public class WarehouseModel : BaseModel
    {
        [Required(AllowEmptyStrings = false)]
        [IndexColumn("IDX_warehouses_name", IsUnique = true)]
        [MaxLength(16)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        [MaxLength(256)]
        public string Description { get; set; }

        public string Notes { get; set; }

        //Collection navigation property
        public ICollection<AreaModel> Areas { get; set; }
    }
}
