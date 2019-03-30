using Moments.dal;
using Moments.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace momentsProject.Controllers
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
                                        where x.username.Equals(usr)
                                        select x).ToList<Notifications>();
            if (nots.Count > 1)
            {
                Debug.WriteLine(nots[0].status);
                Debug.WriteLine(nots[0].uFrom);
            }
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
                status = "Accepted",
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
    }
}