using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Models;

namespace Moments.ModelView
{
    public class AdminsViewModel
    {
        public admins admin { get; set; }
        public List<admins> adminsLst { get; set; }
    }
}