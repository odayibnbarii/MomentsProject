using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class adminPhoto
    {
        [Key]
        [Required]
        [Column(Order = 0)]
        public string type { get; set; }
        [Column(Order = 1)]
        public byte[] image { get; set; }




    }
}