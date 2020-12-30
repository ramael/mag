using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    [Table("loadedcarts")]
    public class LoadedCartModel : BaseModel
    {
        [Required]
        [IndexColumn("IDX_loadedcarts_year", IsUnique = false)]
        [IndexColumn("IDX_loadedcarts_year_progressive", 0, IsUnique = true)]
        public int Year { get; set; }

        [Required(AllowEmptyStrings = false)]
        [IndexColumn("IDX_loadedcarts_progressive", IsUnique = false)]
        [IndexColumn("IDX_loadedcarts_year_progressive", 1, IsUnique = true)]
        [MaxLength(16)]
        public string Progressive { get; set; }

        [Required]
        [ForeignKey("Location")]
        [IndexColumn("IDX_loadedcarts_locationid", IsUnique = false)]
        [IndexColumn("IDX_loadedcarts_locationid_cartid_datein", 0, IsUnique = true)]
        public long LocationId { get; set; }

        //Inverse navigation property
        public LocationModel Location { get; set; }

        [Required]
        [ForeignKey("Cart")]
        [IndexColumn("IDX_loadedcarts_cartid", IsUnique = false)]
        [IndexColumn("IDX_loadedcarts_locationid_cartid_datein", 1, IsUnique = true)]
        public long CartId { get; set; }

        //Inverse navigation property
        public CartModel Cart { get; set; }

        public string Description { get; set; }
        
        [Required]
        [IndexColumn("IDX_loadedcarts_locationid_cartid_datein", 2, IsUnique = true)]
        public DateTime DateIn { get; set; }

        public DateTime? DateOut { get; set; }

        //Collection navigation property
        public ICollection<LoadedCartDetailModel> LoadedCartDetails { get; set; }
    }
}
