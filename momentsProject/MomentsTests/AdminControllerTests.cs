using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moments.Controllers;
using Moments.Models;
using Moments.dal;
using System.Web;

namespace MomentsTests
{
    [TestClass]
    public class AdminControllerTests
    {
        AdminController controller = new AdminController();
        adminsDal adal = new adminsDal();

        [TestMethod]
        public void addNewAdmin() 
        {
            users usr;
            usersDal udal = new usersDal();

            List<admins> alist = (from x in adal.adminsLst where x.username.Equals("test") select x).ToList<admins>();
            if (alist.Count == 1)
            {
                adal.adminsLst.RemoveRange(adal.adminsLst.Where(x => x.username.Equals("test")));
                adal.SaveChanges();
            }
            List<users> list = (from x in udal.userLst where x.username.Equals("test") select x).ToList<users>();
            if (list.Count == 0)
            {
                usr = new users { email = "test@gmail.com", firstName = "test", lastName = "test", username = "test", password = "test" };
                udal.userLst.Add(usr);
                udal.SaveChanges();
            }


            admins adm = new admins { username = "test", aLevel = "A" };
            controller.NewAdmin(adm);

            List<admins> listtest = (from x in adal.adminsLst where x.username.Equals("test") select x).ToList<admins>();
            Assert.AreEqual(listtest.Count, 1);
        }

        [TestMethod]
        public void deleteAdmin()
        {
            adal.adminsLst.RemoveRange(adal.adminsLst.Where(x => x.username.Equals("test")));
            adal.SaveChanges();
            List<admins> list = (from x in adal.adminsLst where x.username.Equals("test") select x).ToList<admins>();

            Assert.AreEqual(list.Count, 0);
        }

        [TestMethod]
        public void getAdmin()
        {
            addNewAdmin();
            var Logged_in_user = "test";

            List<admins> list = (from x in adal.adminsLst where x.username.Equals(Logged_in_user) select x).ToList<admins>();

            Assert.AreEqual(list.Count, 1);

        }

        [TestMethod]
        public void Update_Admin_Level_permition()
        {
            addNewAdmin();
            var admin_to_change = "test";

            adal.adminsLst.RemoveRange(adal.adminsLst.Where(x => x.username.Equals(admin_to_change)));
            adal.SaveChanges();
            admins adm = new admins { username = admin_to_change, aLevel = "B" };
            adal.adminsLst.Add(adm);
            adal.SaveChanges();

            List<admins> list = (from x in adal.adminsLst where x.username.Equals(admin_to_change) select x).ToList<admins>();
            deleteAdmin();
            Assert.AreEqual(list[0].aLevel, "B");
            
        }

        /*[TestMethod]
        public void Delete_exist_User()
        {
            usersDal udal = new usersDal();
            controller.DeleteUser("test");
            List<users> users_list = (from x in udal.userLst where x.username.Equals("test") select x).ToList<users>();
            Assert.AreEqual(users_list.Count, 0);
            
        }*/

        [TestMethod]
        public void Get_Logged_admin_account()
        {
            addNewAdmin();
            var logged_in_admin = "test";

           admins current_admin = controller.GetAdmin(logged_in_admin);

           Assert.AreEqual(current_admin.username, logged_in_admin);
            
        }

        public void delete_test_user()
        {
            usersDal udal = new usersDal();
            List<users> check_user = (from x in udal.userLst where x.username.Equals("test") select x).ToList<users>();
            if (check_user.Count == 1)
            {
                udal.userLst.RemoveRange(udal.userLst.Where(x => x.username.Equals("test")));
                udal.SaveChanges();
            }
        }
        [TestMethod]
        public void get_users_information()
        {
            usersDal udal = new usersDal();
            users usr;
            delete_test_user();
            List<users> list = (from x in udal.userLst select x).ToList<users>();
            int users_number = list.Count;
            

            //Add a new user.
            usr = new users { email = "test@gmail.com", firstName = "test", lastName = "test", username = "test", password = "test" };
            udal.userLst.Add(usr);
            udal.SaveChanges();

            list = (from x in udal.userLst select x).ToList<users>();

            //Check if gett all users correctly!
            Assert.AreEqual(users_number+1, list.Count);
        }

    }
}
