
using Moments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Moments.ModelView
{
    public class MomentsViewModel
    {
        public moments moment { set; get; }
        public List<userMoments> mLst { set; get; }
        
    }
}