using Pdwx.DataObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest
{
    
    
    /// <summary>
    ///This is a test class for PdwReaderTest and is intended
    ///to contain all PdwReaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PdwReaderTest
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
        ///A test for ExtractAll
        ///</summary>
        [TestMethod()]
        public void ExtractAllTest()
        {
            string inputFolder = @"D:\POC\Test\Render\Input\";
            string outputFolder = @"D:\POC\Test\Render\Output\";
            string fileName = "3333";

            string pdwFilePath = inputFolder + fileName + ".pdw";
            PdwReader target = new PdwReader(pdwFilePath);

            outputFolder = outputFolder + fileName + @"\";
            bool isKeppPdwFormat = false;
            target.ExtractAll(outputFolder, isKeppPdwFormat);
        }

        /// <summary>
        ///A test for ExtractDocx
        ///</summary>
        [TestMethod()]
        public void ExtractDocxTest()
        {
            //string pdwFilePath = string.Empty; // TODO: Initialize to an appropriate value
            //PdwReader target = new PdwReader(pdwFilePath); // TODO: Initialize to an appropriate value
            //string outputFolder = string.Empty; // TODO: Initialize to an appropriate value
            //bool isKeepPdwFormat = false; // TODO: Initialize to an appropriate value
            //string expected = string.Empty; // TODO: Initialize to an appropriate value
            //string actual;
            //actual = target.ExtractDocx(outputFolder, isKeepPdwFormat);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
