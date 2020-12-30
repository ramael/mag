using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    [Table("carts")]
    public class CartModel : BaseModel
    {
        public enum StatusEnum
        {
            Available = 0,
            NotAvailable = 1,
            UnderRepair = 2,
            Destroyed = 3
        }

        [Required(AllowEmptyStrings = false)]
        [IndexColumn("IDX_carts_serialnumber", IsUnique = true)]
        [MaxLength(36)]
        public string SerialNumber { get; set; }

        [Required]
        public StatusEnum Status { get; set; }


    }
}
