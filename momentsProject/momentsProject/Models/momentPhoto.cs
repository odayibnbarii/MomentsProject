using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Moments.Models
{
    public class momentPhoto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1)]
        public int postcode { get; set; }
        [Required]
        [Column(Order = 2)]
        public int mId { get; set; }
        [Required]
        [Column(Order = 3)]
        public string username { get; set; }
        [Required]
        [Column(Order = 4)]
        public byte[] photo { get; set; }
    }
}