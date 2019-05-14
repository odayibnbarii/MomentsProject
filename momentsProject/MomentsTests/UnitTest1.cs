using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moments.Controllers;
using Moments.dal;

namespace MomentsTests
{
    [TestClass]
    public class PhotosTest
    {
        MomentController controller = new MomentController();
        publicMomentPhotoDal pdal = new publicMomentPhotoDal();
        momentPhotoDal adal = new momentPhotoDal();
        [TestMethod]
        public void UploadPublicPhoto()
        {

        }
    }
}
