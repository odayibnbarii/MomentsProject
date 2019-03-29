using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Models;


namespace Moments.ModelView
{
    public class UserViewModel
    {
        public users user { get; set; }
        public List<users> users { get; set; }
        public Profile profile { get; set; }
    }
}