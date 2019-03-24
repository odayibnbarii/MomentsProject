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
        [Key, Column(Order = 0)]
        [Required]
        public DateTime dateSent { get; set; }

        [Required][Column(Order = 1)]
        public string username { get; set; }
        [Required]
        [Column(Order = 2)]
        public string type { get; set; }
        [Required]
        [Column(Order = 3)]
        public string uFrom { get; set; }
        [Required]
        [Column(Order = 4)]
        public string status { get; set; }
        [Key,Column(Order = 5)]
        [Required]
        public int id { get; set; }



        
    }
}
    
