using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class Profile
    {
        [Key]
        [Required]
        public string username { get; set; }

        [Required]
        public string biography { get; set; }
        [Required]
        public byte[] image { get; set; }



    }
}