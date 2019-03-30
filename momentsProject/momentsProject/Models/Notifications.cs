using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moments.Models
{
    public class Notifications
    {
        [Key, Column(Order = 5)]
        [Required]
        public int id { get; set; }
        [Required]
        public DateTime dateSent { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public string type { get; set; }
        [Required]
        public string uFrom { get; set; }
        [Required]
        public string status { get; set; }




        
    }
}
    
