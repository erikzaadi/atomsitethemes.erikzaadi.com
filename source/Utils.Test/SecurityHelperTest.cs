using System;
using AtomSite.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Utils.Test
{
    
    
    /// <summary>
    ///This is a test class for SecurityHelperTest and is intended
    ///to contain all SecurityHelperTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SecurityHelperTest
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
        ///A test for HashIt
        ///</summary>
        [TestMethod()]
        public void HashItTest1()
        {
            string input = "All the king's horses and all the king's men.";
            string algorithm = "SHA1";
            bool upperCase = false;
            string expected = "658e306df9ef48f1d837d448fe57ac61f9444089";
            string actual;
            actual = SecurityHelper.HashIt(input, algorithm, upperCase);
            Assert.AreEqual<string>(expected, actual);
            Console.WriteLine(actual);
            Assert.IsTrue(actual.Length == 40);
        }

        /// <summary>
        ///A test for HashIt
        ///</summary>
        [TestMethod()]
        public void HashItTest()
        {
            string input = "All the king's horses and all the king's men.";
            string algorithm = "SHA1";
            string expected = "658E306DF9EF48F1D837D448FE57AC61F9444089";
            string actual;
            actual = SecurityHelper.HashIt(input, algorithm);
            Assert.AreEqual<string>(expected, actual);
            Console.WriteLine(actual);
            Assert.IsTrue(actual.Length == 40);
        }

        /// <summary>
        ///A test for GenerateSymetricKeyTest
        ///</summary>
        [TestMethod()]
        public void GenerateSymetricKeyTest()
        {
            Console.WriteLine(SecurityHelper.GenerateSymmetricKey());
        }

        /// <summary>
        ///A test for GenerateSymetricKeyIv
        ///</summary>
        [TestMethod()]
        public void GenerateSymetricIvTest()
        {
            Console.WriteLine(SecurityHelper.GenerateSymmetricIv());
        }

        /// <summary>
        ///A test for SymetricEncrypt
        ///</summary>
        [TestMethod()]
        public void SymetricEncryptTest()
        {
            byte[] clearBytes = SecurityHelper.HexToByteArray("0123456789ABCDEF");
            byte[] key = SecurityHelper.HexToByteArray("A07C4E1BD5BBFF83BB8D72F2027CD32D077B8C5F7BABC52BD72A277C55943214");
            byte[] iv = SecurityHelper.HexToByteArray("A9DCF37AED8574A1441FD82DB743765A");
            byte[] expected = SecurityHelper.HexToByteArray("871131E5E3A58D2A6032FAE53E606C5B");
            byte[] actual;
            actual = SecurityHelper.SymmetricEncrypt(clearBytes, key, iv);
            Console.WriteLine(SecurityHelper.HexFromByteArray(actual));
            Assert.IsTrue(SecurityHelper.CompareByteArray(actual, expected));
            
        }

        /// <summary>
        ///A test for SymetricDecrypt
        ///</summary>
        [TestMethod()]
        public void SymetricDecryptTest()
        {
            byte[] cipherBytes = SecurityHelper.HexToByteArray("871131E5E3A58D2A6032FAE53E606C5B");
            byte[] key = SecurityHelper.HexToByteArray("A07C4E1BD5BBFF83BB8D72F2027CD32D077B8C5F7BABC52BD72A277C55943214");
            byte[] iv = SecurityHelper.HexToByteArray("A9DCF37AED8574A1441FD82DB743765A");
            byte[] expected = SecurityHelper.HexToByteArray("0123456789ABCDEF");
            byte[] actual;
            actual = SecurityHelper.SymmetricDecrypt(cipherBytes, key, iv);
            Console.WriteLine(SecurityHelper.HexFromByteArray(actual));
            Assert.IsTrue(SecurityHelper.CompareByteArray(actual, expected));
        }

        /// <summary>
        ///A test for PasswordEncrypt
        ///</summary>
        [TestMethod()]
        public void PasswordEncryptTest()
        {
            string clearText = "mysecretpassword12&";
            string key = "A07C4E1BD5BBFF83BB8D72F2027CD32D077B8C5F7BABC52BD72A277C55943214";
            string expected = "60F143C320DAF6F4443F38081D28AAB5C3DDCE081CA113E20FE8EBAE5A458EF3";
            string actual;
            actual = SecurityHelper.PasswordEncrypt(clearText, key);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PasswordDecrypt
        ///</summary>
        [TestMethod()]
        public void PasswordDecryptTest()
        {
            string cipherText = "60F143C320DAF6F4443F38081D28AAB5C3DDCE081CA113E20FE8EBAE5A458EF3";
            string key = "A07C4E1BD5BBFF83BB8D72F2027CD32D077B8C5F7BABC52BD72A277C55943214";
            string expected = "mysecretpassword12&"; 
            string actual;
            actual = SecurityHelper.PasswordDecrypt(cipherText, key);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
            
        }
    }
}
