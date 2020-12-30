using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    [Table("areas")]
    public class AreaModel : BaseModel
    {
        [Required(AllowEmptyStrings = false)]
        [IndexColumn("IDX_areas_name", IsUnique = true)]
        [MaxLength(16)]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = true)]
        [MaxLength(256)]
        public string Description { get; set; }

        public string Notes { get; set; }

        [Required]
        [ForeignKey("Warehouse")]
        [IndexColumn("IDX_areas_warehouseid", IsUnique = false)]
        public long WarehouseId { get; set; }

        //Inverse navigation property
        public WarehouseModel Warehouse { get; set; }

        //Collection navigation property
        public ICollection<LocationModel> Locations { get; set; }
    }
}
