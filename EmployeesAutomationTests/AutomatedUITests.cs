using FluentAssertions;
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
        public void When_DataIsCorrectAndNoSolution_ThenNoMatchMessage()
        {
            // Arrange
            var filePath = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\TestFiles\test1_no_match.txt"
                ));

            // Act
            _driver.Navigate()
                .GoToUrl("http://localhost:53685");

            // Selenuim doesn't support handling modal dialogs so I am not going to open the FileOpenDialog here. 
            // Instead I am going to directly add file path with the command below.
            //_driver.FindElement(By.Name("browse"))
            //    .Click();

            _driver.FindElement(By.Id("FileUpload_FileData"))
                .SendKeys(filePath);

            _driver.FindElement(By.Name("upload"))
                .Click();

            // Assert
            _driver.FindElement(By.Id("no_match")).Text.Trim().Length
                .Should().BeGreaterThan(0);
        }


        [Theory]
        [InlineData("test2.txt", 222, 333, 13, 2720)]
        [InlineData("test3.txt", 222, 333, 13, 2720)]
        [InlineData("test4.txt", 333, 433, 13, 2751)]
        //TODO implementation for DateTo = NULL test cases - every day correct value can change
        public void When_DataIsCorrectAndThereIsSolution_ThenCheckEndResult(string inputFile, int employeeID1, int employeeID2, int projectID, int daysWorked)
        {
            // Arrange
            var filePath = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\TestFiles\" + inputFile
                ));

            // Act
            _driver.Navigate()
                .GoToUrl("http://localhost:53685");

            // Selenuim doesn't support handling modal dialogs so I am not going to open the FileOpenDialog here. 
            // Instead I am going to directly add file path with the command below.
            //_driver.FindElement(By.Name("browse"))
            //    .Click();

            _driver.FindElement(By.Id("FileUpload_FileData"))
                .SendKeys(filePath);

            _driver.FindElement(By.Name("upload"))
                .Click();

            // Assert
            int.Parse(_driver.FindElement(By.Id("EmployeeID1")).Text.Trim())
                .Should().Be(employeeID1);
            int.Parse(_driver.FindElement(By.Id("EmployeeID2")).Text.Trim())
                .Should().Be(employeeID2);
            int.Parse(_driver.FindElement(By.Id("ProjectID")).Text.Trim())
                .Should().Be(projectID);
            int.Parse(_driver.FindElement(By.Id("DaysWorked")).Text.Trim())
                .Should().Be(daysWorked);
        }

        [Theory]
        [InlineData("test5_incorrect_data.txt")]
        [InlineData("test6_incorrect_data.txt")]
        [InlineData("test7_incorrect_data.txt")]
        [InlineData("test8_incorrect_data.txt")]
        [InlineData("test9_incorrect_data.txt")]
        public void When_DataIsNotCorrectAndThereIsSolution_ThenErrorMessage(string inputFile)
        {
            // Arrange
            var filePath = Path.GetFullPath(Path.Combine(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                @"..\..\..\TestFiles\" + inputFile
                ));

            // Act
            _driver.Navigate()
                .GoToUrl("http://localhost:53685");

            // Selenuim doesn't support handling modal dialogs so I am not going to open the FileOpenDialog here. 
            // Instead I am going to directly add file path with the command below.
            //_driver.FindElement(By.Name("browse"))
            //    .Click();

            _driver.FindElement(By.Id("FileUpload_FileData"))
                .SendKeys(filePath);

            _driver.FindElement(By.Name("upload"))
                .Click();

            // Assert
            _driver.FindElement(By.Id("error_message")).Text.Trim().Length
                .Should().BeGreaterThan(0);
        }
    }
}
