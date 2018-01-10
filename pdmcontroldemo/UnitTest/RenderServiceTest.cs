using Pdwx.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Pdwx.DataObjects;
using Pdwx;
using System.Xml;

namespace UnitTest
{


    /// <summary>
    ///This is a test class for RenderServiceTest and is intended
    ///to contain all RenderServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RenderServiceTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Render
        ///</summary>
        [TestMethod()]
        public void RenderTest()
        {
            //string fileName = "6874b728-d1a6-441a-8e50-aafc43b0fb84";
            //string folder = @"D:\POC\Test\Render\Output\d12\";

            //string pdwr = string.Format("{0}{1}.pdwr", folder, fileName);
            //string xml = string.Format("{0}{1}.xml", folder, fileName);
            //string xsl = string.Format("{0}{1}.xsl", folder, fileName);

            //EmailInfo email = new EmailInfo();
            //email.To.Add("nqtngoc@yahoo.com");
            //email.Subject = "test mwxml";
            //email.Body1 = "test attach mwxml";

            //RenderService target = new RenderService();
            //XmlDocument xDoc = new XmlDocument();
            //xDoc.Load(xml);
            //RenderInfo renderInfo = new RenderInfo(
            //    pdwr,
            //    RenderMode.Normal,
            //    Utilities.ReadFileContent(xsl),
            //    xDoc,
            //    @"D:\POC\Test\Render\Input\i01.pdw",
            //    new RenderSettings() { Channel = RenderSettings.ChannelType.Attachment, Media = RenderSettings.MediaType.MsXml, ChannelSpecificInfo = email });

            //target.Render(renderInfo);
        }
    }
}
