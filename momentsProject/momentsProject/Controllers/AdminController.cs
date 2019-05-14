using System;
using System.Collections.Generic;
using System.IO;
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



        public admins GetAdmin(string admin = "")
        {
            string currentuser;
            try
            {
                currentuser = Session["CurrentUsername"].ToString();
            }
            catch (Exception)
            {
                currentuser = admin;
            }
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
        public ActionResult defaultPhoto()
        {
            return View();
        }

        public ActionResult NewAdmin(admins admin)
        {


            List<users> lst = (from x in udal.userLst where x.username == admin.username select x).ToList<users>();
            List<admins> admins = (from x in adal.adminsLst where x.username == admin.username select x).ToList<admins>();
            if (lst.Count > 0 && admins.Count == 0)
            {

                adal.adminsLst.Add(admin);
                adal.SaveChanges();
                ViewBag.MESSAGE = "successfully added";
            }
            else if (admins.Count > 0)
            {
                TempData["ADDERROR"] = "This user is already an admin!";
            }
            else
            {
                TempData["ADDERROR"] = "Username not found!";
            }
            return RedirectToAction("adminsList", "Admin");
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
            return RedirectToAction("searchByUsrName", "User", " ");
        }

        public ActionResult usersList()
        {
            UserViewModel uvm = new UserViewModel();
            uvm.users = (from x in udal.userLst select x).ToList<users>();
            uvm.admin = GetAdmin();

            return View(uvm);

        }

        public ActionResult SearchUser()
        {
            String toSearch = Request.Form["username"];
            UserViewModel uvm = new UserViewModel();
            uvm.users = (from x in udal.userLst where x.username.StartsWith(toSearch) select x).ToList<users>();
            uvm.admin = GetAdmin();
            return View("usersList", uvm);
            
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

        public ActionResult DeleteUser(string delete)
        {
            profileDal pdal = new profileDal();
            UsersStatusDal usdal = new UsersStatusDal();
            notificationsDal ndal = new notificationsDal();
            friendsDal fdal = new friendsDal();
            String toDelete;
            try
            {
                toDelete = Request.Form["row"].ToString();
            }
            catch (Exception)
            {
                toDelete = delete;
            }

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

            return RedirectToAction("usersList", "Admin");
        }
        public void changeDefault(HttpPostedFileBase file)
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
                Profile profile = new Profile();
                data = memorystram.ToArray();
                profile.username = "testImage";
                profile.biography = "empty";
                profile.image = data;
                pdal.profilesList.RemoveRange(pdal.profilesList.Where(x => x.username.Equals("testImage")));
                pdal.profilesList.Add(profile);
                try
                {

                    pdal.SaveChanges();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
        public ActionResult MessageView()
        {
            return View("NotificationView");
        }
        public ActionResult SendMessage()
        {
            string message = Request.Form["mess"].ToString();
            notificationsDal dal = new notificationsDal();
            Notifications n = new Notifications();
            n.username = "Admin";
            n.id = (from x in dal.nLst select x).ToList<Notifications>().Count() + 1;
            n.status = message;
            n.dateSent = DateTime.Now.Date;
            n.type = "Admin Message";
            n.uFrom = "Admin";
            try
            {
                dal.nLst.Add(n);
                dal.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ViewData["ErrorMessage"] = "Error While sending message, try again in a moment";
            }
            return View("NotificationView");

        }
        public ActionResult photoChange()
        {
            return View("photoChange");
        }
        public ActionResult changeLogo(HttpPostedFileBase file)
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
                websiteImages dal = new websiteImages();
                adminPhoto p = new adminPhoto();
                int size = (from x in dal.pLst
                            where x.type.Equals("Logo")
                            select x).ToList<adminPhoto>().Count();
                if(size > 0)
                {
                    dal.pLst.RemoveRange(dal.pLst.Where(x => x.type.Equals("Logo")));
                    try
                    {
                        dal.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("photoChange");
                    }
                }
                data = memorystram.ToArray();
                p.type = "Logo";
                p.image = data;
                dal.pLst.Add(p);
                try
                {
                    dal.SaveChanges();
                }
                catch(Exception e)
                {
                    
                }

            }
            Moments.Controllers.HomeController.LoadLogo();
            return RedirectToAction("photoChange");
        }
        public ActionResult changeBack(HttpPostedFileBase file)
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
                websiteImages dal = new websiteImages();
                int size = (from x in dal.pLst
                            where x.type.Equals("Background")
                            select x).ToList<adminPhoto>().Count();
                if (size > 0)
                {
                    dal.pLst.RemoveRange(dal.pLst.Where(x => x.type.Equals("Background")));
                    try
                    {
                        dal.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        return RedirectToAction("photoChange");
                    }
                }
                adminPhoto p = new adminPhoto();
                data = memorystram.ToArray();
                p.type = "Background";
                p.image = data;
                dal.pLst.Add(p);
                try
                {
                    dal.SaveChanges();
                }
                catch (Exception e)
                {

                }



            }
            return RedirectToAction("photoChange");
        }
        public ActionResult Statistics()
        {

            return View();
        }
        public ActionResult Statistics1()
        {

            return View();
        }
    }
}
