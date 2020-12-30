using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    [Table("loadedcartdetails")]
    public class LoadedCartDetailModel : BaseModel
    {

        [Required]
        [ForeignKey("LoadedCart")]
        [IndexColumn("IDX_loadedcartdetails_loadedcartid", IsUnique = false)]
        public long LoadedCartId { get; set; }

        //Inverse navigation property
        public LoadedCartModel LoadedCart { get; set; }

        [Required]
        [ForeignKey("Component")]
        [IndexColumn("IDX_loadedcartdetails_componentid", IsUnique = false)]
        public long ComponentId { get; set; }

        //Inverse navigation property
        public ComponentModel Component { get; set; }

        public string Notes { get; set; }

    }
}
