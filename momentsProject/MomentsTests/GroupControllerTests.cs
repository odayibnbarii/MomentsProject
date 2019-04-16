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
    public class GroupControllerTests
    {
        userMomentDal usermomdal = new userMomentDal();
        momentsDal momdal = new momentsDal();
        userMoments test = new userMoments();
        moments mtest = new moments();

        [TestMethod]
        public void Delete_User_From_Group()
        {

            test.username = "test";
            test.id = 10;
            test.mid = -1;
            test.uType = "User";
            usermomdal.userMomentLST.Add(test);
            usermomdal.SaveChanges();
            usermomdal.userMomentLST.RemoveRange(usermomdal.userMomentLST.Where(x => (x.username.Equals("test") && x.mid.Equals(-1))));
            usermomdal.SaveChanges();
            List<userMoments> list = (from x in usermomdal.userMomentLST where x.username.Equals("test") select x).ToList<userMoments>();
            Assert.AreEqual(list.Count, 0);
        }
        [TestMethod]
        public void Delete_Group_Moments()
        {
            var authToken = new byte[16];
            byte[] Momentimage = authToken;
            mtest.mid = -1;
            mtest.mImage = Momentimage;
            mtest.mDescription = "stam to test";
            mtest.mName = "First Moment";
            momdal.momentsLst.Add(mtest);
            momdal.SaveChanges();
            momdal.momentsLst.RemoveRange(momdal.momentsLst.Where(x => x.mid.Equals(-1)));
            momdal.SaveChanges();
            List<moments> list = (from x in momdal.momentsLst where x.mid.Equals(-1) select x).ToList<moments>();
            Assert.AreEqual(list.Count, 10);
        }
    }
}
