using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Toolbelt.ComponentModel.DataAnnotations.Schema.V5;

namespace MagApi.Models
{
    public class BaseModel
    {
        public long Id { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        public DateTime CreatedOn 
        { 
            get { return this.createdon.HasValue ? this.createdon.Value : DateTime.Now; }
            set { this.createdon = value; } 
        }

        private DateTime? createdon = null;

        [Required]
        public string ModifiedBy { get; set; }

        public DateTime ModifiedOn 
        {
            get { return this.modifiedon.HasValue ? this.modifiedon.Value : DateTime.Now; }
            set { this.modifiedon = value; }
        }
        private DateTime? modifiedon = null;

    }
}
