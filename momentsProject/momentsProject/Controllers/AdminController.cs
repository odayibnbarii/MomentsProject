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
            return RedirectToAction("searchByUsrName", "User");
        }
    }
}