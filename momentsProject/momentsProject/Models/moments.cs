using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Moments.Models
{
    public class moments
    {
        [Key]
        [Required]
        public int mid { get; set; }

        [Required]
        public string mName { get; set; }

        [Required]
        public string mDescription { get; set; }

        [Required]
        public byte[] mImage { get; set; }
        [Required]
        public int IsPublic { get; set; }
        [Required]
        public DateTime dateCreate { get; set; }
        public moments()
        {
            IsPublic = 1;
            dateCreate = DateTime.Now;
        }

    }
}