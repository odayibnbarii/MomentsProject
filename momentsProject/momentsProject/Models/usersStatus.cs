using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Moments.Models
{
    public class usersStatus
    {

        [Key]
        [Required]
        public String username { get; set; }

        [Required]
        public String status { get; set; }

    }
}