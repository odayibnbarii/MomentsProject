using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class Friends
    {
        [Key]
        [Required]
        [Column(Order = 0)]
        public string username { get; set; }
        [Key]
        [Required]
        [Column(Order = 1)]
        public string friendUsername { get; set; }




    }
}