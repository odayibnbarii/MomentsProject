using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Moments.Controllers;
using Moments.Models;
using Moments.dal;
namespace MomentsTests
{
    [TestClass]
    public class HomeControllerTests
    {
        HomeController controller = new HomeController();
    
        [TestMethod]
        public void Avout_View()
        {
            ViewResult result = controller.About() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void register_View()
        {
            ViewResult result = controller.Register() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Login_exist_user()
        {
            ViewResult result = controller.Login() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void  new_user_Register()
        {
            users usr = new users { email = "test@gmail.com", firstName = "test", lastName = "test", username = "test", password = "test" };
            usersDal udal = new usersDal();
            udal.userLst.RemoveRange(udal.userLst.Where(x => x.username.Equals(usr.username)));
            udal.userLst.Add(usr);
            udal.SaveChanges();
            List<users> database = (from x in udal.userLst where x.username == usr.username select x).ToList<users>();
           
            Assert.AreEqual(database.Count, 1);
            
        }

     
    }
}
