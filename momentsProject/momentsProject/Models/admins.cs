using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moments.Models
{
    public class admins
    {
        [Key]
        [Required]
        public string username { get; set; }

        [Required]
        public string aLevel { get; set; }
    }
}