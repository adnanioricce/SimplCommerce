using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace SimplCommerce.IntegrationTests
{
    [TestClass]
    public class EdgeDriverTest
    {        
        private EdgeDriver _driver;

        [TestInitialize]
        public void EdgeDriverInitialize()
        {
            // Initialize edge driver 
            var options = new EdgeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal,
                BrowserVersion = "84.0.522.49"                
                
            };            
            _driver = new EdgeDriver(options);
        }

        [TestMethod]
        public void VerifyPageTitle()
        {            
            _driver.Url = "http://localhost:49208";
            var title = _driver.Title;
            Assert.AreEqual("localhost", title);
        }

        [TestCleanup]
        public void EdgeDriverCleanup()
        {
            _driver.Quit();
        }
    }
}
