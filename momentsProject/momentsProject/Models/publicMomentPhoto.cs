using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class publicMomentPhoto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int postcode { get; set; }
        [Required]
        [Column(Order = 2)]
        public string username { get; set; }
        [Required]
        [Column(Order = 3)]
        public byte[] photo { get; set; }
    }
}