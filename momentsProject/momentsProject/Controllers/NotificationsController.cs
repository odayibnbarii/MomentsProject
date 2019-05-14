using Moments.dal;
using Moments.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Moments.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        public ActionResult Index()
        {
            return RedirectToAction("showNotifications", "Notifications");
        }

        public ActionResult showNotifications()
        {
            string usr = Session["CurrentUsername"].ToString();
            notificationsDal nDal = new notificationsDal();
            List<Notifications> nots = (from x in nDal.nLst
                                        where x.username.Equals(usr) || x.username.Equals("Admin")
                                        select x).ToList<Notifications>();
            if (nots.Count > 1)
            {
                Debug.WriteLine(nots[0].status);
                Debug.WriteLine(nots[0].uFrom);
            }
            nots.OrderByDescending(x => x.id);
            return View(nots);
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
                status = "Return Accepted",
                username = u2,
                uFrom = u1
            };
            nDal.nLst.Add(n);
            nDal.SaveChanges();

            return RedirectToAction("showNotifications", "Notifications");

        }

        public ActionResult searchByUsrName()
        {
            return RedirectToAction("searchByUsrName", "User");
        }

        public ActionResult inviteAccepted()
        {
            string username = Request.Form["username"];
            string uFrom = Request.Form["uFrom"];
            int id = Convert.ToInt32(Request.Form["id"]);
            Notifications n = new Notifications();
            notificationsDal nDal = new notificationsDal();
            int size = (from x in nDal.nLst select x).ToList<Notifications>().Count() + 1;
            List<Notifications> tmp = (from x in nDal.nLst
                                       where x.id == id
                                       select x).ToList<Notifications>();
            nDal.nLst.RemoveRange(nDal.nLst.Where(x => x.id == id));
            nDal.SaveChanges();
            tmp[0].status = "Accepted";
            nDal.nLst.Add(tmp[0]);
            nDal.SaveChanges();
            n.id = id;
            n.dateSent = DateTime.Now.Date;
            n.status = "Return Accepted";
            n.type = tmp[0].type;
            n.uFrom = username;
            n.username = uFrom;
            nDal.nLst.Add(n);
            nDal.SaveChanges();
            userMomentDal mDal = new userMomentDal();
            userMoments uM = new userMoments();
            int mid = Convert.ToInt32((tmp[0].type.Split(' '))[1]);
            int tableId = (from x in mDal.userMomentLST
                           select x).ToList<userMoments>().Count() + 1;
            momentsDal d = new momentsDal();
            string groupName = (from x in d.momentsLst
                                where x.mid == mid
                                select x).ToList<moments>()[0].mName;
            uM.id = tableId;
            uM.mid = mid;
            uM.GroupName = groupName;
            uM.uType = "User";
            uM.username = username;
            mDal.userMomentLST.Add(uM);
            mDal.SaveChanges();
            return RedirectToAction("showNotifications", "Notifications");

        }
        
        public ActionResult joinAccepted()
        {
            string username = Request.Form["username"];
            string uFrom = Request.Form["uFrom"];
            int id = Convert.ToInt32(Request.Form["id"]);
            Notifications n = new Notifications();
            notificationsDal nDal = new notificationsDal();
            int size = (from x in nDal.nLst select x).ToList<Notifications>().Count() + 1;
            List<Notifications> tmp = (from x in nDal.nLst
                                       where x.id == id
                                       select x).ToList<Notifications>();
            nDal.nLst.RemoveRange(nDal.nLst.Where(x => x.id == id));
            nDal.SaveChanges();
            tmp[0].status = "Accepted";
            nDal.nLst.Add(tmp[0]);
            nDal.SaveChanges();
            n.id = id;
            n.dateSent = DateTime.Now.Date;
            n.status = "Return Accepted";
            n.type = tmp[0].type;
            n.uFrom = username;
            n.username = uFrom;
            nDal.nLst.Add(n);
            nDal.SaveChanges();
            int mid = Convert.ToInt32((tmp[0].type.Split(' '))[1]);
            userMomentDal mDal = new userMomentDal();
            userMoments uM = new userMoments();
            momentsDal d = new momentsDal();
            string groupName = (from x in d.momentsLst
                                where x.mid == mid
                                select x).ToList<moments>()[0].mName;
            int tableId = (from x in mDal.userMomentLST
                           select x).ToList<userMoments>().Count() + 1;
            uM.id = tableId;
            uM.GroupName = groupName;
            uM.mid = mid;
            uM.uType = "User";
            uM.username = uFrom;
            mDal.userMomentLST.Add(uM);
            mDal.SaveChanges();

            return RedirectToAction("showNotifications", "Notifications");
        }
    }
}