using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Moments.Models;

namespace Moments.ModelView
{
    public class MomentPhotoView
    {
        public string photo { get; set; }
        public List<momentPhoto> momentphotos { get; set; }
    }
}