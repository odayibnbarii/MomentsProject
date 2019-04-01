using Moments.dal;
using Moments.Models;
using Moments.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moments.Controllers
{
    public class MomentController : Controller
    {
        // GET: Moment
        public ActionResult Index()
        {
            return View();
        }
        public static string getMomentTitle(string invitation)
        {
            List<string> splitted = invitation.Split(' ').ToList<string>();
            momentsDal mDal = new momentsDal();
            int mid = Convert.ToInt32(splitted[1]);
            List<moments> m = (from x in mDal.momentsLst
                               where x.mid == mid
                               select x).ToList<moments>();
            if (m.Count > 0)
            {
                return m[0].mName;
            }return "Error occured";

        }
        public static bool isMember(string username, int mid)
        {
            userMomentDal mDal = new userMomentDal();
            List<userMoments> tmp = (from x in mDal.userMomentLST
                                     where x.mid == mid &&
                                            x.username.Equals(username)
                                     select x).ToList<userMoments>();
            if(tmp.Count > 0)
            {
                return true;
            }
            return false;
        }
        /*
        public ActionResult UserMoments()
        {
            users user = UserController.user;
            userMomentDal uMDal = new userMomentDal();
            List<userMoments> myMoments = (from x in uMDal.userMomentLST
                                           where x.username == user.username
                                           select x).ToList<userMoments>();
            momentsDal mDal = new momentsDal();
            List<moments> allMoments = (from x in mDal.momentsLst
                                        select x).ToList<moments>();
            List<MomentsViewModel> user_momentsTOView = new List<MomentsViewModel>();
            List<moments> tmp = new List<moments>();
            foreach(userMoments u in myMoments)
            {
                MomentsViewModel model = new MomentsViewModel();
                model.moment = (from x in mDal.momentsLst
                                where u.mid == x.mid
                                select x).ToList<moments>()[0];
                model.mLst = (from x in uMDal.userMomentLST
                              where u.mid == x.mid
                              select x).ToList<userMoments>();
                user_momentsTOView.Add(model);
            }
            return View("UserMoments", user_momentsTOView);
        }
        
        public ActionResult searchMoment()
        {
            
            string text = Request.Form["searchText"];
            momentsDal mDal = new momentsDal();
            List<moments> result = (from x in mDal.momentsLst
                                where x.mName.StartsWith(text)
                                select x).ToList<moments>();
            return View(result);
            

        }*/

    }
}