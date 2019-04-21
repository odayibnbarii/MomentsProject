using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class Messages
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string message { get; set; }
        [Required]
        public int mid { get; set; }
        [Required]
        public string username { get; set; }

    }
}