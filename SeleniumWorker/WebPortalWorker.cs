using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;

namespace SeleniumWorker
{
    public class WebPortalWorker
    {
        private const string WebPortalUrl = "https://webportal.gfk.com/BG/A/Account#/Login";
        private IWebDriver _driver;
        private bool _userIsLoggedIn;

        public WebPortalWorker()
        {
        }

        public static WebDriverWait GlobaWait { get; set; }

        /// <summary>
        /// Fills in time-sheet data for the selected period for a single work item
        /// </summary>
        public void FillInPeriod(DateTime startDate, DateTime endDate, WorkItem workItem)
        {
            if (!_userIsLoggedIn)
            {
                throw new InvalidOperationException("No logged in user. Call Login method first");
            }

            Thread.Sleep(7000);

            //if (endDate.Date <= startDate.Date)
            //{
            //    // Create today 
            //    _driver.FindElement(By.XPath(" .//*[@id='CreateStart-button']")).Click();
            //    Thread.Sleep(4000);

            //    FillInDay(workItem);
            //}

            try
            {
                // create for range
                while (startDate.Date <= endDate.Date)
                {
                    _driver.FindElement(By.XPath(".//*[@id='Create-button']")).Click();
                    Thread.Sleep(3000);
                    _driver.FindElement(By.XPath(".//*[@id='dtp_TimeRecordingEntries_DocumentDate']"))
                        .SendKeys(startDate.Date.ToString("MM/dd/yyyy"));
                    Thread.Sleep(3000);
                    _driver.FindElement(By.XPath(".//*[@id='LinesForm']")).Click();

                    Thread.Sleep(3000);

                    FillInDay(workItem);

                    startDate = startDate.AddDays(1);
                }
            }
            catch (NoSuchElementException nex)
            {
                throw new InvalidOperationException("The element was not found. " + nex.Message, nex.InnerException);
            }
        }

        private void FillInDay(WorkItem workItem)
        {
            _driver.FindElement(By.XPath(".//*[@id='TimeRecordingEntries_WorkType']/div/input")).SendKeys(workItem.WorkTypeCode.ToString());
            _driver.FindElement(By.XPath(".//*[@id='TimeRecordingEntries_WorkType']/div/input")).SendKeys(Keys.Enter);
            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='TimeRecordingEntries_Hours']/div/input")).Click();
            Thread.Sleep(1000);
            _driver.FindElement(By.XPath(".//*[@id='TimeRecordingEntries_Hours']/div/input")).SendKeys(Keys.Control + "a");
            Thread.Sleep(1000);
            _driver.FindElement(By.XPath(".//*[@id='TimeRecordingEntries_Hours']/div/input")).SendKeys(workItem.WorkHours.ToString());
            _driver.FindElement(By.XPath(".//*[@id='TimeRecordingEntries_Hours']/div/input")).SendKeys(Keys.Enter);
            Thread.Sleep(2000);

            string dropDownId;
            string inputVal;

            if (workItem.WorkTypeCode < 200)
            {
                dropDownId = ".//*[@id='TimeRecordingEntries_ShortcutDimension2Code']/div";
                inputVal = workItem.CostUnitCode.ToString();
            }
            else
            {
                dropDownId = ".//*[@id='TimeRecordingEntries_ShortcutDimension1Code']/div";
                inputVal = workItem.CostCenterCode.ToString();
            }

            _driver.FindElement(By.XPath(dropDownId)).Click();
            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='select2-drop']/div/input")).SendKeys(inputVal);
            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='select2-drop']/div/input")).SendKeys(Keys.Enter);
            Thread.Sleep(3000);
        }

        public void Logout()
        {
            if (!_userIsLoggedIn)
            {
                return;
            }

            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='scrollContainer']/ul/li[9]/a")).Click();
            Thread.Sleep(1000);

            _userIsLoggedIn = false;
            _driver.Dispose();
            _driver.Quit();
        }

        public void Login(string userName, string password)
        {
            if (_userIsLoggedIn)
            {
                return;
            }

            _driver = new InternetExplorerDriver(Path.Combine(Environment.CurrentDirectory, "lib"));
            _driver.Navigate().GoToUrl(WebPortalUrl);

            GlobaWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20));
            // Log in to Web Portal
            Thread.Sleep(7000);
            _driver.FindElement(By.XPath(".//*[@id='Username']/div/input")).SendKeys(userName);
            _driver.FindElement(By.XPath(".//*[@id='Password']/div/input")).SendKeys(password);
            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='Password']/div/input")).SendKeys(Keys.Enter);

            _userIsLoggedIn = true;
        }

    }
}
