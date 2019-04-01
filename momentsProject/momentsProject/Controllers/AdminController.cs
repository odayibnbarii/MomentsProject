using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Moments.dal;
using Moments.Models;
using Moments.ModelView;

namespace Moments.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private adminsDal adal = new adminsDal();
        private usersDal udal = new usersDal();
        private UsersStatusDal usdal = new UsersStatusDal();
        private AdminsViewModel avm = new AdminsViewModel();

 

        public admins GetAdmin()
        {
            string currentuser = Session["CurrentUsername"].ToString();
            List<admins> curr = (from x in adal.adminsLst
                                where x.username == currentuser
                                select x).ToList<admins>();
            return curr[0];   
        }

        public ActionResult adminsList()
        {
            avm.adminsLst = (from x in adal.adminsLst select x).ToList<admins>();
            avm.admin = GetAdmin();
            return View("AdminsList", avm);

        }

        public ActionResult NewAdmin(admins admin) 
        {
    
            
            List<users> lst = (from x in udal.userLst where x.username == admin.username select x).ToList<users>();
            List<admins> admins = (from x in adal.adminsLst where x.username == admin.username select x).ToList<admins>();
            if (lst.Count > 0  && admins.Count == 0)
            {
                
                adal.adminsLst.Add(admin);
                adal.SaveChanges();
            }
            else if (admins.Count >0)
            {
                TempData["ADDERROR"] = "This user is already an admin!";
            }
            else
            {
                TempData["ADDERROR"] = "Username not found!";
            }
            return RedirectToAction("adminsList","Admin");
        }

        public ActionResult deleteAdmin()
        {
            String toDelete = Request.Form["rowDelete"].ToString();
            List<admins> lst = (from x in adal.adminsLst where x.username.Equals(toDelete) select x).ToList<admins>();
            adal.adminsLst.RemoveRange(adal.adminsLst.Where(x => x.username == toDelete));
            adal.SaveChanges();
            return RedirectToAction("adminsList", "Admin");
        }

        public ActionResult ChangeAdminLevel()
        {

            String toChange = Request.Form["row"].ToString();
            admins admin = new admins();
            admin.username = toChange;
            if (Request.Form["toDo"].Equals("promote"))
            {
                admin.aLevel = "A";
            }
            else
            {
                admin.aLevel = "B";
            }
            adal.adminsLst.RemoveRange(adal.adminsLst.Where(x => x.username == toChange));
            adal.SaveChanges();
            adal.adminsLst.Add(admin);
            adal.SaveChanges();

            return RedirectToAction("adminsList", "Admin");

        }

        public ActionResult searchByUsrName()
        {
            return RedirectToAction("searchByUsrName", "User"," ");
        }

        public ActionResult usersList()
        {
            UserViewModel uvm = new UserViewModel();
            uvm.users = (from x in udal.userLst select x).ToList<users>();
            uvm.admin = GetAdmin();

            return View(uvm);

        }

        public ActionResult UserActivity()
        {
            String toChange = Request.Form["row"].ToString();
            usersStatus us = new usersStatus();
            us.username = toChange;

            if (Request.Form["toDo"].Equals("Activate"))
                us.status = "Activated";
            else
                us.status = "Deactivated";

            usdal.statuses.RemoveRange(usdal.statuses.Where(x => x.username == toChange));
            usdal.SaveChanges();
            usdal.statuses.Add(us);
            usdal.SaveChanges();

            return RedirectToAction("usersList", "Admin");
        }

        public ActionResult DeleteUser()
        {
            profileDal pdal = new profileDal();
            UsersStatusDal usdal = new UsersStatusDal();
            notificationsDal ndal = new notificationsDal();
            friendsDal fdal = new friendsDal();

            String toDelete = Request.Form["row"].ToString();
            udal.userLst.RemoveRange(udal.userLst.Where(x => x.username.Equals(toDelete)));
            adal.adminsLst.RemoveRange(adal.adminsLst.Where(x => x.username.Equals(toDelete)));
            pdal.profilesList.RemoveRange(pdal.profilesList.Where(x => x.username.Equals(toDelete)));
            usdal.statuses.RemoveRange(usdal.statuses.Where(x => x.username.Equals(toDelete)));
            ndal.nLst.RemoveRange(ndal.nLst.Where(x => x.username.Equals(toDelete) || x.uFrom.Equals(toDelete)));
            fdal.FriendsLst.RemoveRange(fdal.FriendsLst.Where(x => x.username.Equals(toDelete)));
            fdal.FriendsLst.RemoveRange(fdal.FriendsLst.Where(x => x.friendUsername.Equals(toDelete)));

            udal.SaveChanges();
            adal.SaveChanges();
            pdal.SaveChanges();
            usdal.SaveChanges();
            ndal.SaveChanges();
            fdal.SaveChanges();

            return RedirectToAction("usersList","Admin");
        }
    }
}