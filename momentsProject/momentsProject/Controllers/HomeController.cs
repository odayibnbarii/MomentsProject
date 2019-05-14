using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Moments.Models;
using Moments.dal;


namespace Moments.Controllers
{
    
    public class HomeController : Controller
    {
        public static byte[] image;
        public ActionResult Index()
        {
            TempData["LogMess"] = "";
            if (Session["connected"] == null)
            {
                Session["connected"] = "0";
            }
            websiteImages dal = new websiteImages();
            List<adminPhoto> ps = (from x in dal.pLst
                                   where x.type.Equals("Background")
                                   select x).ToList<adminPhoto>();
            return View(ps);
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }
        public ActionResult Login()
        {
            usersDal adal = new usersDal();
            adal.SaveChanges();
            return View();
        }
        public ActionResult RegAction(users userReg)
        {
            usersDal emails = new usersDal();
            profileDal pdal = new profileDal();
            UsersStatusDal usdal = new UsersStatusDal();
            List<users> checkLSt = (from tmp in emails.userLst where tmp.email.Contains(userReg.email) select tmp).ToList<users>();
            if (checkLSt.Count > 0)
            {
                ViewData["eMess"] = "This e-mail has registered before\n try another one.";
            }
            else
            {

                if (userReg.password != Request.Form["rePass"])
                {
                    ViewData["pMess"] = "The passwords doesn't match";
                }
                else if(userReg.password.Length < 6)
                {
                    ViewData["pMess"] = "The password length must be greater than 6";
                }
                else
                {
                    Profile profile = new Profile();
                    usersStatus uss = new usersStatus();
                    List<Profile> defProfile = (from x in pdal.profilesList
                                                where x.username.Equals("testImage")
                                                select x).ToList<Profile>();
                    emails.userLst.Add(userReg);
                    emails.SaveChanges();
                    profile.username = userReg.username;
                    profile.biography = "empty";
                    profile.image = defProfile[0].image;
                    pdal.profilesList.Add(profile);
                    uss.username = userReg.username;
                    uss.status = "Activated";
                    usdal.statuses.Add(uss);
                    usdal.SaveChanges();
                    
                    try
                    {

                        pdal.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    return View("Login");
                }
            }
            return View("Register");
        }

        public ActionResult LogAction(users info)
        {
            usersDal usrInfo = new usersDal();
            adminsDal admin = new adminsDal();
            UsersStatusDal usdal = new UsersStatusDal();
            List<users> user = (from usr in usrInfo.userLst
                                where usr.username.Equals(info.username)
                                  && usr.password.Equals(info.password)
                                select usr).ToList<users>();
            List<admins> adminsLst = (from x in admin.adminsLst
                                      where x.username.Equals(info.username)
                                      select x).ToList<admins>();
            List<usersStatus> statuslist = (from x in usdal.statuses
                                            where x.username.Equals(info.username)
                                            select x).ToList<usersStatus>();
           
            Session["Admin"] = "False";
            if (adminsLst.Count() == 1)
            {
                Session["admin"] = "True";
            }
            if (user.Count() == 1)
            {
                if (statuslist[0].status.Equals("Deactivated"))
                {
                    TempData["LogMess"] = "your account deactivated by admins!";
                    return View("Login");
                }
                Session["CurrentUsername"] = info.username;
                Session["firstName"] = user[0].firstName;
                Session["lastName"] = user[0].lastName;
                Session["connected"] = "1";     
                return RedirectToAction("UserMainPage", "User");
            }
            TempData["LogMess"] = "The username or the password isn't correct";
            return View("Login");
        }
        public ActionResult checkConnected()
        {
            if (Session["connected"] != null && Session["connected"].ToString().Equals("1"))
            {
                return RedirectToAction("MyMoments", "Moment");
            }
            else
            {
                return View("Login");
            }
        }
        public void add(HttpPostedFileBase file)
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
        public static void LoadLogo()
        {
            websiteImages dal = new websiteImages();
            List<adminPhoto> lst = (from x in dal.pLst
                                    where x.type.Equals("Logo")
                                    select x).ToList<adminPhoto>();
            try
            {
                image = lst[0].image;
            }catch(Exception e)
            {

            }
        }



    }
}

        
        