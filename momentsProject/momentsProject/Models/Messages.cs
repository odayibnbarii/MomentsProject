using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace momentsProject.Models
{
    public class Messages
    {
        [Key]
        [Required]
        public int id;
        [Required]
        public int mid { get; set; }
        [Required]
        public string message { get; set; }
    }
}