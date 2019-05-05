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
using System.Web.Routing;

namespace Moments.Controllers
{
    public class UserController : Controller
    {
        userMomentDal umd = new userMomentDal();

        public static users user
        {
            get;
            set;
        }
        // GET: User
        public ActionResult UserMainPage()
        {
            GetUser();
            return RedirectToAction("PublicPhoto", "Moment");
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
            classActive("profileActive");
            UserViewModel uvm = new UserViewModel();
            uvm.user = GetUser();
            uvm.profile = GetProfile();
            return View(uvm);

        }
        public ActionResult MyFriends()
        {
            classActive("friendsActive");
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
            return View("MyFriends", profiles);


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
            newNoti.dateSent = DateTime.Now;
            newNoti.type = "Friend Request";
            newNoti.id = size + 1;

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
            return View("MyFriends", profiles);
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
            string getthemaxnumber = Request.Form["Momentmaxnumber"].ToString();
            Session["getthemaxnumber"] = getthemaxnumber;
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
            usermoment.GroupName = MomentName;
            Session["MomentName1"] = MomentName;
            Session["getmidd"] = usermoment.mid;
            usermoment.uType = "Admin";
            umd.userMomentLST.Add(usermoment);
            umd.SaveChanges();

            return View("AddMembersToGroup");
        }
        public JsonResult CheckLogInInformation(string username1)
        {

            usersDal db = new usersDal();
            userMomentDal umd = new userMomentDal();

            //Momentmaxnumber

            string getmax = Session["getthemaxnumber"].ToString();
            int value = Convert.ToInt32(getmax);


            
                

            int getmidnumber = Convert.ToInt32(Session["getmidd"]);

            System.Threading.Thread.Sleep(2000);
            var GetIfUserExist = db.userLst.Where(x => x.username == username1).SingleOrDefault();
            List<userMoments> usersingroup = (from tmp in umd.userMomentLST where tmp.mid.Equals(getmidnumber) select tmp).ToList<userMoments>();
            if (usersingroup.Count() <= value)
            {
                if (GetIfUserExist != null)
                {
                    users oh = new users();
                    oh = GetUser();
                    if (username1 != oh.username)
                    {
                        Notifications n = new Notifications();
                        notificationsDal nDal = new notificationsDal();
                        int id = (from x in nDal.nLst
                                  select x).ToList<Notifications>().Count() + 1;
                        n.dateSent = DateTime.Now.Date;
                        n.id = id;
                        n.status = "Not Accepted";
                        n.type = "invite " + Session["mid"];
                        n.username = username1;
                        n.uFrom = oh.username;
                        nDal.nLst.Add(n);
                        nDal.SaveChanges();

                        userMoments usermoment = new userMoments();
                        usermoment.id = 1;
                        usermoment.mid = Convert.ToInt32(Session["mid"]);
                        usermoment.GroupName = Session["MomentName1"].ToString();
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
            else
            {
                return Json(3);
            }

        }



        public ActionResult UserMoments()
        {
            classActive("momentsActive");
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
        public ActionResult View_All_Group_Members(int mid1)
        {
            profileDal pDal = new profileDal();
            userMomentDal usermdal = new userMomentDal();
            var qr = (from x in usermdal.userMomentLST where x.mid == mid1 select x.username).ToArray();
            List<Profile> allprofiles = (from x in pDal.profilesList select x).ToList<Profile>();
            List<userMoments> AllUsers = (from x in usermdal.userMomentLST select x).ToList<userMoments>();
            List<Profile> selected = new List<Profile>();
            List<userMoments> usersinthisgroup = new List<userMoments>();

            var size = qr.Count();

            foreach (var z in AllUsers)
            {
                for (int v = 0; v < size; v++)
                {
                    if (z.username == qr[v])
                    {
                        usersinthisgroup.Add(z);
                    }
                }
            }
            Session["UsersList"] = usersinthisgroup;
            foreach (var z in allprofiles)
            {
                for (int v = 0; v < size; v++)
                {
                    if (z.username == qr[v])
                    {
                        selected.Add(z);
                    }
                }
            }
            Session["SelectedList"] = selected;
            return View(selected);
        }
        public ActionResult ExitGroup(int id, int mid1)
        {
            users corruser = new users();
            corruser = GetUser();
            userMomentDal usermdal = new userMomentDal();
            momentsDal momentsmdal = new momentsDal();

            var getifuserUser = usermdal.userMomentLST.Where(x => (corruser.username == x.username)
            && x.uType == "User" && id == x.id).SingleOrDefault();


            var getifuseradmin = usermdal.userMomentLST.Where(x => (corruser.username == x.username)
            && x.uType == "Admin" && id == x.id).SingleOrDefault();

            /* List<userMoments> allmomentsusehave = (from tmp in dusernames.userMomentLST
                                                  where corruser.username.Equals(tmp.username)
                                                  select tmp).ToList<userMoments>();*/


            if (getifuseradmin != null)
            {
                List<userMoments> allmomentsusehave = (from tmp in usermdal.userMomentLST
                                                       where id.Equals(tmp.mid)
                                                       select tmp).ToList<userMoments>();

                usermdal.userMomentLST.RemoveRange(usermdal.userMomentLST.Where(x => x.mid == mid1));
                usermdal.SaveChanges();
                momentsmdal.momentsLst.RemoveRange(momentsmdal.momentsLst.Where(x => x.mid == mid1));
                momentsmdal.SaveChanges();
            }
            else if (getifuserUser != null)
            {
                usermdal.userMomentLST.RemoveRange(usermdal.userMomentLST.Where(x => (x.mid == mid1) && (x.username == corruser.username)));
                usermdal.SaveChanges();
            }
            return View("UserMainPage");
        }
        public ActionResult GroupEditName(int mid2)
        {
            Session["correntMid"] = mid2;
            return View("EditGroupName");
        }

        public ActionResult EditGroupName()
        {
            
            var getgroupnewname = Request.Form["editgroupname"].ToString();
            int mid2 = Convert.ToInt32(Session["correntMid"]);
            momentsDal md = new momentsDal();
            userMomentDal umd = new userMomentDal();
            List<moments> litsofallmd = (from tmp in md.momentsLst where tmp.mid.Equals(mid2) select tmp).ToList<moments>();
            var toedit = md.momentsLst.Where(f => f.mid.Equals(mid2)).ToList();
            if (litsofallmd.Count > 0)
            {
                if (litsofallmd[0].mName != getgroupnewname)
                {
                    toedit.ForEach(a => a.mName = getgroupnewname);
                    md.SaveChanges();
                    List<userMoments> membersofthisgroup = (from tmp in umd.userMomentLST where tmp.mid.Equals(mid2) select tmp).ToList<userMoments>();
                    foreach (var x in membersofthisgroup )
                    {
                        umd.userMomentLST.Remove(x);
                        userMoments toadd1 = new userMoments();
                        toadd1.id = x.id;
                        toadd1.GroupName = getgroupnewname;
                        toadd1.mid = x.mid;
                        toadd1.username = x.username;
                        toadd1.uType = x.uType;
                        umd.userMomentLST.Add(toadd1);
                        umd.SaveChanges();
                    }
                    TempData["ErrorMessageEdit"] = "Your Group Name Has Been Edited Sucessfully";
                    return View("EditGroupName");
                }
            }
            {
                TempData["ErrorMessageEdit"] = "You Have A Problem With Group Name , Please Enter New Name";
                return View("EditGroupName");
            }
        }
       
        public moments getcorrentrungroup()
        {
            moments getcorrgroup = new moments();

            return getcorrgroup;
        } 

        public ActionResult DeleteFriend(string username)
        {
            List<userMoments> myList = (List<userMoments>)Session["UsersList"];
            List<Profile> SELECTEDLIST = (List<Profile>)Session["SelectedList"];
            List<Profile> templist = SELECTEDLIST;
            userMomentDal usermdal = new userMomentDal();
            profileDal pDal = new profileDal();
            var idtodelete = 0;
             
            foreach (var x in myList)
            {
                if (x.username == username)
                {
                    idtodelete = x.id;
                }
            }
            var ifuserexit = usermdal.userMomentLST.Where(x => (username == x.username)
            && x.uType == "User" && idtodelete == x.id).SingleOrDefault();

            if (ifuserexit != null)
            {
                usermdal.userMomentLST.RemoveRange(usermdal.userMomentLST.Where(x => (x.id == idtodelete) && (x.username == username)));
                usermdal.SaveChanges();
                Profile todelete = templist.Find(x=> x.username == username);
                templist.Remove(todelete);
            }
            else
            {
                TempData["CurrentMessage"] = "You Cant Delete Your Self From This Group!";
                return View("View_All_Group_Members", SELECTEDLIST); 
            }
            return View("View_All_Group_Members", templist);
        }
       


        public ActionResult SaveMoment()
        {
            return RedirectToAction("UserMoments", "User");
        }

        public static bool isFriend(string id)
        {
            bool friendCheck = false;
            friendsDal fDal = new friendsDal();
            List<Friends> f = (from x in fDal.FriendsLst
                               where x.username.Equals(user.username) || x.friendUsername.Equals(user.username)
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
            ViewData["momentsActive"] = "#";
            ViewData["profileActive"] = "#";
            ViewData["friendsActive"] = "#";
            ViewData[tab] = "active";

        }
        public ActionResult searchMoment()
        {
            string text = Request.Form["text"];
            momentsDal mDal = new momentsDal();
            List<moments> result = (from x in mDal.momentsLst
                                    where x.mName.StartsWith(text)
                                    select x).ToList<moments>();
            return View(result);
        }
        public ActionResult ShowAllGroups()
        {
            momentsDal mDal = new momentsDal();
            List<moments> result = (from x in mDal.momentsLst select x).ToList<moments>();
            return View("searchMoment", result);
        }
            public ActionResult JoinRequest()
        {
            userMomentDal mDal = new userMomentDal();
            int mid = Convert.ToInt32(Request.Form["mid"]);
            List<userMoments> findAdmin = (from x in mDal.userMomentLST
                                           where x.mid == mid &&
                                                  x.uType.Equals("Admin")
                                           select x).ToList<userMoments>();
            string adminUsr = findAdmin[0].username;
            notificationsDal nDal = new notificationsDal();
            Notifications n = new Notifications();
            n.uFrom = UserController.user.username;
            n.username = adminUsr;
            n.dateSent = DateTime.Now.Date;
            n.id = (from x in nDal.nLst select x).ToList<Notifications>().Count + 1;
            n.type = "Join " + mid;
            n.status = "Not Accepted";
            nDal.nLst.Add(n);
            nDal.SaveChanges();
            return RedirectToAction("UserMoments", "User");
        }

        void clear_data_from_database()
        {
            userMomentDal d = new userMomentDal();
            momentsDal d1 = new momentsDal();
            usersDal user1 = new usersDal();
            foreach (var entity in d.userMomentLST)
                d.userMomentLST.Remove(entity);
            d.SaveChanges();
            foreach (var entity in d1.momentsLst)
                d1.momentsLst.Remove(entity);
            d1.SaveChanges();
            foreach (var entity in user1.userLst)
                user1.userLst.Remove(entity);
            user1.SaveChanges();

        }
        public ActionResult MomentView()
        {

            MomentPhotoView mpv = new MomentPhotoView();
            momentPhotoDal mp = new momentPhotoDal();
            List<momentPhoto> curr = new List<momentPhoto>();
            int mid;
            try
            {
                mid = int.Parse(Request.Form["mid"].ToString());
            }catch(Exception e)
            {
                mid = int.Parse(Session["LastmMid"].ToString());
            }
            int momentId = mid;
            Session["LastmMid"] = mid;



            curr = (from x in mp.momentPhotoLst
                    where momentId.Equals(x.mId)
                    select x).ToList<momentPhoto>();


            mpv.momentphotos = curr;
            mpv.photo = "moment";

            return View(mpv);
        }
        public ActionResult DeleteMomentPhoto()
        {
            int postcode = int.Parse(Request.Form["postcode"].ToString());
            string usrname = Request.Form["user"].ToString();
            momentPhotoDal dal = new momentPhotoDal();
            dal.momentPhotoLst.RemoveRange(dal.momentPhotoLst.Where(x => x.postcode == postcode));
            try
            {
                dal.SaveChanges();
            }
            catch (Exception e)
            {

            }
            return RedirectToAction("MomentView", "User");
        }

    }
}
 