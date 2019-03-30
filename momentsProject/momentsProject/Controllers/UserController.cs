using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Moments.dal;
using Moments.Models;
using Moments.ModelView;
using System.IO;
using System.Web;
using System;
using System.Diagnostics;

namespace Moments.Controllers
{
    public class UserController : Controller
    {
        static List<users> toView = new List<users>();
        userMomentDal umd = new userMomentDal();

        public static users user
        {
            get;
            set;
        }
        // GET: User
        public ActionResult UserMainPage()
        {
            return View();
        }


        public ActionResult Logout()
        {
            user = null;
            Session["connected"] = "0";
            Session["admin"] = null;
            //  MomentController.m = new List<moments>();
            return RedirectToAction("Index", "Home");
        }



        public users GetUser()
        {
            usersDal dal = new usersDal();
            string currentuser = Session["CurrentUsername"].ToString();
            List<users> curr = (from x in dal.userLst
                                where x.username == currentuser
                                select x).ToList<users>();
            user = curr[0];
            return curr[0];
        }
        public Profile GetProfile()
        {
            profileDal dal = new profileDal();
            string currentuser = Session["CurrentUsername"].ToString();
            List<Profile> curr = (from x in dal.profilesList where x.username == currentuser select x).ToList<Profile>();
            return curr[0];
        }

        public ActionResult MyProfile()
        {
            UserViewModel uvm = new UserViewModel();
            uvm.user = GetUser();
            uvm.profile = GetProfile();
            return View(uvm);

        }
        public ActionResult MyFriends()
        {
            friendsDal fDal = new friendsDal();
            String uname = Session["CurrentUsername"].ToString();
            Debug.WriteLine(uname);
            List<Friends> friends1 = (from x in fDal.FriendsLst
                                      where x.username.Equals(uname) 
                                      select x).ToList<Friends>();
            
            List<Friends> friends2 = (from x in fDal.FriendsLst
                                      where x.friendUsername.Equals(uname)
                                     select x).ToList<Friends>();
            IEnumerable<Friends> friendAll = friends1.Concat<Friends>(friends2);
            List<String> friends = new List<string>();
            foreach (var f in friendAll)
            {
                if (f.username.Equals(uname))
                    friends.Add(f.friendUsername);
                else
                    friends.Add(f.username);
            }
            //Debug.WriteLine(friends[0]);
            //IEnumerable<Friends> friends = friends1.Union(friends2);
            profileDal pDal = new profileDal();
            List<Profile> profiles = (from x in pDal.profilesList
                                      where friends.Contains<String>(x.username)
                                            select x).ToList<Profile>();
            return View(profiles);
        }
        public ActionResult Delete(String id)
        {
            String uname = Session["CurrentUsername"].ToString();
            friendsDal fDal = new friendsDal();
            List<Friends> friendsd = (from x in fDal.FriendsLst
                                      where (x.username.Equals(uname) && x.friendUsername.Equals(id))
                                     || (x.friendUsername.Equals(uname) && x.username.Equals(id))
                                      select x).ToList<Friends>();
            fDal.FriendsLst.Remove(friendsd[0]);
            fDal.SaveChanges();
            List<String> friends1 = (from x in fDal.FriendsLst
                                     where x.username.Equals(uname)
                                     select x.friendUsername).ToList<String>();

            List<String> friends2 = (from x in fDal.FriendsLst
                                     where x.friendUsername.Equals(uname)
                                     select x.username).ToList<String>();
            IEnumerable<String> friends = friends1.Union(friends2);
            profileDal pDal = new profileDal();
            List<Profile> profiles = (from x in pDal.profilesList
                                      where friends.Contains<String>(x.username)
                                      select x).ToList<Profile>();
            return View("MyFriends",profiles);


        }

        public ActionResult editProfile()
        {
            string firstname = Request.Form["firstName"].ToString();
            string lastname = Request.Form["lastName"].ToString();
            string bio = Request.Form["biography"].ToString();
            string pass = Request.Form["password"].ToString();
            string repass = Request.Form["repassword"].ToString();
            Profile oldDBProfile = GetProfile();
            users oldDBUser = GetUser();
            oldDBUser.firstName = firstname;
            oldDBUser.lastName = lastname;
            oldDBProfile.biography = bio;
            if (pass != "" && pass == repass)
                oldDBUser.password = pass;
            else if (pass != "" && pass != repass)
                TempData["ERROR"] = "Unmatched Passwords! , try again.";

            usersDal udal = new usersDal();
            profileDal pdal = new profileDal();
            udal.userLst.RemoveRange(udal.userLst.Where(x => x.username == oldDBUser.username));
            udal.SaveChanges();
            udal.userLst.Add(oldDBUser);
            udal.SaveChanges();
            pdal.profilesList.RemoveRange(pdal.profilesList.Where(x => x.username == oldDBProfile.username));
            pdal.SaveChanges();
            pdal.profilesList.Add(oldDBProfile);
            pdal.SaveChanges();
            return RedirectToAction("MyProfile", "User");
        }
        public ActionResult editProfilePhoto(HttpPostedFileBase file)
        {
            byte[] data;

            using (Stream inputStram = file.InputStream)
            {
                MemoryStream memorystram = inputStram as MemoryStream;
                if (memorystram == null)
                {
                    memorystram = new MemoryStream();
                    inputStram.CopyTo(memorystram);

                }
                profileDal pdal = new profileDal();
                Profile profile = GetProfile();
                data = memorystram.ToArray();
                profile.image = data;
                pdal.profilesList.RemoveRange(pdal.profilesList.Where(x => x.username == profile.username));
                pdal.SaveChanges();
                pdal.profilesList.Add(profile);
                try
                {
                    Console.WriteLine("DONE");
                    pdal.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
            return RedirectToAction("MyProfile", "User");
        }

        public ActionResult showNotifications()
        {
            string usr = Session["CurrentUsername"].ToString();
            notificationsDal nDal = new notificationsDal();
            List<Notifications> nots = (from x in nDal.nLst
                                        where x.username.Equals(usr)
                                        select x).ToList<Notifications>();
            if (nots.Count > 1)
            {
                Debug.WriteLine(nots[0].status);
                Debug.WriteLine(nots[0].uFrom);
            }
            return View(nots);
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
        public ActionResult AddFriendAccepted()
        {
            string u1 = Request.Form["u1"].ToString();
            string u2 = Request.Form["u2"].ToString();
            int id = Convert.ToInt32(Request.Form["id"]);
            friendsDal fDal = new friendsDal();
            List<Friends> checkFriends = (from x in fDal.FriendsLst
                                          where (x.username.Equals(u1) && x.friendUsername.Equals(u2)) ||
                                                (x.username.Equals(u2) && x.friendUsername.Equals(u1))
                                                select x).ToList<Friends>();
            Friends f = new Friends();
            f.username = u1;
            f.friendUsername = u2;
            fDal.FriendsLst.Add(f);
            fDal.SaveChanges();
            Notifications n = new Notifications();
            notificationsDal nDal = new notificationsDal();
            List<Notifications> nots = (from x in nDal.nLst select x).ToList();
            List<Notifications> prev = (from x in nDal.nLst
                                        where x.id == id
                                        select x).ToList<Notifications>();
            nDal.nLst.RemoveRange(nDal.nLst.Where(x => x.id == id));
            nDal.SaveChanges();
            n.id = id;
            n.status = "Accepted";
            n.type = "Friend Request";
            n.username = u1;
            n.uFrom = u2;
            n.dateSent = DateTime.Now;
            n.dateSent = n.dateSent.Date;
            nDal.nLst.Add(n);
            nDal.SaveChanges();
            n = new Notifications
            {
                dateSent = DateTime.UtcNow,
                id = nots.Count() + 1,
                type = "Friend Request",
                status = "Accepted",
                username = u2,
                uFrom = u1
            };
            nDal.nLst.Add(n);
            nDal.SaveChanges();

            return RedirectToAction("showNotifications", "User");

        }
        public ActionResult AddFriend(String id)
        {
            notificationsDal nDal = new notificationsDal();
            List<Notifications> tmp = (from x in nDal.nLst
                                       select x).ToList<Notifications>();
            int size = tmp.Count();
            Notifications newNoti = new Notifications();
            newNoti.username = id;
            newNoti.status = "Not Accepted";
            newNoti.uFrom = Session["CurrentUsername"].ToString();
            newNoti.dateSent=  DateTime.Now;
            newNoti.type = "Friend Request";
            newNoti.id = size+1;
        
            nDal.nLst.Add(newNoti);
            nDal.SaveChanges();


            friendsDal fDal = new friendsDal();
            String uname = Session["CurrentUsername"].ToString();
            List<Friends> friends1 = (from x in fDal.FriendsLst
                                      where x.username.Equals(uname)
                                      select x).ToList<Friends>();

            List<Friends> friends2 = (from x in fDal.FriendsLst
                                      where x.friendUsername.Equals(uname)
                                      select x).ToList<Friends>();
            IEnumerable<Friends> friendAll = friends1.Union(friends2);
            List<String> friends = new List<string>();
            foreach (var f in friendAll)
            {
                if (f.username.Equals(uname))
                    friends.Add(f.friendUsername);
                else
                    friends.Add(f.username);
            }
            
            //IEnumerable<Friends> friends = friends1.Union(friends2);
            profileDal pDal = new profileDal();
            List<Profile> profiles = (from x in pDal.profilesList
                                      where friends.Contains<String>(x.username)
                                      select x).ToList<Profile>();
            return View("MyFriends",profiles);
        }
        






        public ActionResult CreateGroup()
        {
            return View("GroupCreate");
        }
        public ActionResult AddMembersToGroup()
        {
            return View("AddMembersToGroup");
        }

        public ActionResult RunCreateGroup(moments obj)
        {
            int counter;
            string MomentName = Request.Form["MomentName"].ToString();
            string MomentDescription = Request.Form["MomentDescription"].ToString();
            var authToken = new byte[16];
            byte[] Momentimage = authToken;

            userMomentDal usrmomentsdal = new userMomentDal();
            momentsDal moments = new momentsDal();

            List<moments> momentsID = (from tmp in moments.momentsLst select tmp).ToList<moments>();
            
            if (momentsID.Count == 0)
            {
                counter = 1;
            }
            else
            {
                counter = moments.momentsLst.Max(x => x.mid) + 1;
            }

            moments added_moment = new moments();
            added_moment.mid = 1;
            added_moment.mImage = Momentimage;
            added_moment.mName = MomentName;
            added_moment.mDescription = MomentDescription;
            moments.momentsLst.Add(added_moment);
            moments.SaveChanges();

            users u = GetUser();
            userMoments usermoment = new userMoments();
            usermoment.username = u.username;
            Session["mid"] = counter;
            usermoment.mid = counter;
            usermoment.uType = "Admin";
            umd.userMomentLST.Add(usermoment);
            umd.SaveChanges();

            return View("AddMembersToGroup");
        }
        public JsonResult CheckLogInInformation(string username1)
        {

            usersDal db = new usersDal();

            System.Threading.Thread.Sleep(2000);
            var GetIfUserExist = db.userLst.Where(x => x.username == username1).SingleOrDefault();
            if (GetIfUserExist != null)
            {
                users oh = new users();
                oh = GetUser();
                if (username1 != oh.username)
                {
                    userMoments usermoment = new userMoments();
                    usermoment.id = 1;
                    usermoment.mid = Convert.ToInt32(Session["mid"]);
                    usermoment.username = username1;
                    usermoment.uType = "User";
                    umd.userMomentLST.Add(usermoment);
                    umd.SaveChanges();
                    return Json(1);
                }
                else
                {
                    return Json(2);
                }

            }
            else
            {
                return Json(0);
            }
        }
        public ActionResult UserMoments()
        {
            users corruser = new users();
            corruser = GetUser();
            userMomentDal dusernames = new userMomentDal();
            momentsDal md = new momentsDal();
            userMoments useromoentsobj = new userMoments();

            momentsDal moments = new momentsDal();

            List<userMoments> allmomentsusehave = (from tmp in dusernames.userMomentLST
                                                   where corruser.username.Equals(tmp.username)
                                                   select tmp).ToList<userMoments>();

            return View(allmomentsusehave);
        }
        public ActionResult ExitGroup(int id)
        {
            users corruser = new users();
            corruser = GetUser();
            userMomentDal usermdal = new userMomentDal();
            momentsDal momentsmdal = new momentsDal();
             var getifuserUser = usermdal.userMomentLST.Where(x => (corruser.username == x.username)
                                                                && x.uType == "User").SingleOrDefault();
            var getifuseradmin = usermdal.userMomentLST.Where(x => (corruser.username == x.username) && x.uType == "Admin").SingleOrDefault();

            /* List<userMoments> allmomentsusehave = (from tmp in dusernames.userMomentLST
                                                  where corruser.username.Equals(tmp.username)
                                                  select tmp).ToList<userMoments>();*/


            if (getifuseradmin != null)
            {
                List<userMoments> allmomentsusehave = (from tmp in usermdal.userMomentLST
                                                       where id.Equals(tmp.mid)
                                                       select tmp).ToList<userMoments>();

                usermdal.userMomentLST.RemoveRange(usermdal.userMomentLST.Where(x => x.mid == id));
                usermdal.SaveChanges();
                momentsmdal.momentsLst.RemoveRange(momentsmdal.momentsLst.Where(x => x.mid == id));
                momentsmdal.SaveChanges();
            }
            else if (getifuserUser != null)
            {
                usermdal.userMomentLST.RemoveRange(usermdal.userMomentLST.Where(x => (x.mid == id) && (x.username == corruser.username)));
                usermdal.SaveChanges();
            }
            return View("UserMainPage");
        }
        public static bool isFriend(string id)
        {
            bool friendCheck = false;
            friendsDal fDal = new friendsDal();
            List<Friends> f = (from x in fDal.FriendsLst
                               where x.username.Equals(id) || x.friendUsername.Equals(id)
                               select x).ToList<Friends>();
            List<Friends> tmp = new List<Friends>();
            foreach (Friends fr in f)
            {
                if (fr.username.Equals(id) || fr.friendUsername.Equals(id))
                {
                    tmp.Add(fr);
                }
            }
            if (tmp.Count() > 0)
            {
                friendCheck = true;
            }
            return friendCheck;

        }
        public ActionResult RedirectToProfile()
        {
            string usr = Request.Form["usr"].ToString();
            usersDal uDal = new usersDal();
            UserViewModel view = new UserViewModel();
            profileDal pDal = new profileDal();
            List<Profile> profile = (from x in pDal.profilesList
                                     where x.username.Equals(usr)
                                     select x).ToList<Profile>();
            List<users> u = (from x in uDal.userLst
                             where x.username.Equals(usr)
                             select x).ToList<users>();

            view.profile = profile[0];
            view.user = u[0];
            return View(view);
            //return RedirectToAction("ViewFriendProfile", "Profile", usrname);
        }
        private void classActive(string tab)
        {
            ViewData["myMoments"] = "#";
            ViewData["myProfile"] = "#";
            ViewData["myFriends"] = "#";
            ViewData[tab] = "active";

        }


    }
}
 