using Moments.dal;
using Moments.Models;
using Moments.ModelView;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace momentsProject.Controllers
{
    public class MomentController : Controller
    {
        private MomentPhotoView mpv { get; set; }
        
        private int mid { get; set; }
        
        // GET: Moment
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MomentView(int id)
        {
            mpv = new MomentPhotoView();
            momentPhotoDal mp = new momentPhotoDal();
            List<momentPhoto> curr=new List<momentPhoto>();
            int momentId=id;
            mid = id;
            Session["LastmMid"] = id;



            curr = (from x in mp.momentPhotoLst
                           where  momentId.Equals(x.mId)
                        select x).ToList<momentPhoto>();
          
            
            mpv.momentphotos = curr;
            mpv.photo = "moment";

            return View(mpv);
        }
        public ActionResult PublicPhoto()
        {
            classActive("publicActive");
            friendsDal fd = new friendsDal();
            string username = Session["CurrentUsername"].ToString();
            List<Friends> curr = (from x in fd.FriendsLst
                                  where (x.username.Equals(username)) || (x.friendUsername.Equals(username))
                                  select x).ToList<Friends>();
            List<string> friendsUserName = new List<string>();
            friendsUserName.Add(username);
            foreach (var friend in curr)
            {
                if (friend.username.Equals(username))
                {
                    friendsUserName.Add(friend.friendUsername);
                }
                else
                {
                    friendsUserName.Add(friend.username);
                }

            }
            PublicMomentPhotoView pmpv = new PublicMomentPhotoView();
            publicMomentPhotoDal pmpd = new publicMomentPhotoDal();
            List<publicMomentPhoto> photos= (from x in pmpd.momentPhotoLst
                                             where   friendsUserName.Any(item =>item.Equals(x.username))
                                             select x).ToList<publicMomentPhoto>();
            pmpv.publicMomentphotos = photos;
            pmpv.photo = "photo";
            return View(pmpv);
        }
        [HttpPost]
        public ActionResult AddPublicPhoto(IEnumerable<HttpPostedFileBase>
            imageModel)
        {
            byte[] data;
            using (Stream inputStram = Request.Files[0].InputStream)
            {
                MemoryStream memorystram = inputStram as MemoryStream;
                if (memorystram == null)
                {
                    memorystram = new MemoryStream();
                    inputStram.CopyTo(memorystram);

                }
                publicMomentPhotoDal mpd1 = new publicMomentPhotoDal();
                publicMomentPhoto mp = new publicMomentPhoto();
                data = memorystram.ToArray();
                mp.photo = data;
                mp.username = Session["CurrentUsername"].ToString();
                mpd1.momentPhotoLst.Add(mp);
                mpd1.SaveChanges();
                ViewData["photo"] = "Photo Added";


            }
            friendsDal fd = new friendsDal();
            string username = Session["CurrentUsername"].ToString();
            List<Friends> curr = (from x in fd.FriendsLst
                                  where (x.username.Equals(username)) || (x.friendUsername.Equals(username))
                                  select x).ToList<Friends>();
            List<string> friendsUserName = new List<string>();
            friendsUserName.Add(username);
            foreach (var friend in curr)
            {
                if (friend.username.Equals(username))
                {
                    friendsUserName.Add(friend.friendUsername);
                }
                else
                {
                    friendsUserName.Add(friend.username);
                }

            }
            PublicMomentPhotoView pmpv = new PublicMomentPhotoView();
            publicMomentPhotoDal pmpd = new publicMomentPhotoDal();
            List<publicMomentPhoto> photos = (from x in pmpd.momentPhotoLst
                                              where friendsUserName.Any(item => item.Equals(x.username))
                                              select x).ToList<publicMomentPhoto>();
            pmpv.publicMomentphotos = photos;
            pmpv.photo = "photo";
            return View("PublicPhoto", pmpv);

        }
        public ActionResult searchByUsrName(String text)
        {
            //search profile, not user
            profileDal pDal = new profileDal();

            string usrName;
            try
            {
                usrName = Request.Form["text"].ToString();
            }
            catch
            {
                usrName = text;
            }
            List<Profile> profiles = (from x in pDal.profilesList
                                      where x.username.StartsWith(usrName)
                                      select x).ToList<Profile>();
            return View(profiles);
        }
        [HttpPost]
        public ActionResult AddPhoto(IEnumerable<HttpPostedFileBase>
            imageModel)
        {
            byte[] data;
            using (Stream inputStram = Request.Files[0].InputStream)
            {
                MemoryStream memorystram = inputStram as MemoryStream;
                if (memorystram == null)
                {
                    memorystram = new MemoryStream();
                    inputStram.CopyTo(memorystram);

                }
                momentPhotoDal mpd1 = new momentPhotoDal();
                momentPhoto mp=new momentPhoto();
                data = memorystram.ToArray();
                mp.mId = int.Parse(Session["LastmMid"].ToString());
                mp.photo = data;
                mp.username = Session["CurrentUsername"].ToString();
                mpd1.momentPhotoLst.Add(mp);
                mpd1.SaveChanges();
                ViewData["photo"] = "Photo Added";


            }
            mpv = new MomentPhotoView();
            momentPhotoDal mp1 = new momentPhotoDal();
            List<momentPhoto> curr = new List<momentPhoto>();
            int momentId =int.Parse( Session["LastmMid"].ToString());
            



            curr = (from x in mp1.momentPhotoLst
                    where momentId.Equals(x.mId)
                    select x).ToList<momentPhoto>();


            mpv.momentphotos = curr;
            mpv.photo = "moment";

            return View("MomentView",mpv );


        }
        private void classActive(string tab)
        {
            ViewData["momentsActive"] = "#";
            ViewData["profileActive"] = "#";
            ViewData["friendsActive"] = "#";
            ViewData[tab] = "active";

        }
    }
}
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