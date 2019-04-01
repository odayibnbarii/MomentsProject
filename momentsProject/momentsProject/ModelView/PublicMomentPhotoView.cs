using Moments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.ModelView
{
    public class PublicMomentPhotoView
    {
        public string photo { get; set; }
        public List<publicMomentPhoto> publicMomentphotos { get; set; }
    }
}