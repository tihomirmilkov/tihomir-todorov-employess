using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace EmployeesAutomationTests
{
public class AutomatedUITests : IDisposable
    {
        private readonly IWebDriver _driver;
        public AutomatedUITests()
        {
            _driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Fact]
        public void Create_WrongModelData_ReturnsErrorMessage()
        {
            var filePath = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\TestFiles\test1.txt"
                ));

            _driver.Navigate()
                .GoToUrl("http://localhost:53685");

            _driver.FindElement(By.Name("browse"))
                .Click();

            _driver.FindElement(By.Id("file_input"))
                .SendKeys(filePath);

            //_driver.FindElement(By.Id("Age"))
            //    .SendKeys("34");

            //_driver.FindElement(By.Id("Create"))
            //    .Click();

            //var errorMessage = _driver.FindElement(By.Id("AccountNumber-error")).Text;

            //Assert.Equal("Account number is required", errorMessage);
        }
    }
}
