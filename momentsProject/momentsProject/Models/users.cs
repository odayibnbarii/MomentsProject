using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class users
    {
        [Required]
        public String firstName { get; set; }
        [Required]
        public String lastName { get; set; }
        [Key]
        [Required]
        public string username { get; set; }
        [Required]
        public String email { get; set; }
        [Required]
        public String password { get; set; }


    }
}