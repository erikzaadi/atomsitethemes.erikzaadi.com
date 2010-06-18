using System;
using AtomSite.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Utils.Test
{
    
    
    /// <summary>
    ///This is a test class for StringHelperTest and is intended
    ///to contain all StringHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringHelperTest
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
        ///A test for AbbreviateText
        ///</summary>
        [TestMethod()]
        public void AbbreviateTextTest()
        {
            string input = "All the king's horses and all the king's men.";
            int length = 30; 
            string trailer = "...";
            string expected = "All the king's horses...";
            string actual; 
            actual = StringHelper.AbbreviateText(input, length, trailer);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
