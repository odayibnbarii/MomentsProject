using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Moments.dal
{
    public class userMoments
    {
        [Key]
        [Required]
        public int id { get; set; }
        [Required]
        public string username { get; set; }
        [Required]
        public int mid { get; set; }
        [Required]
        public string uType { get; set; }
    }
}