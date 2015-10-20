﻿using System;
using System.Collections.Generic;
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
        private readonly string WebPortalUrl = "https://webportal.gfk.com/BG/A/Account#/Login";

        private readonly IWebDriver _driver;

        public WebPortalWorker()
        {
            _driver = new InternetExplorerDriver();
        }

        public static WebDriverWait GlobaWait { get; set; }

        /// <summary>
        /// Fills in time-sheet data for the selected period for a single work item
        /// </summary>
        public void FillInPeriod(string userName, string password, DateTime startDate, DateTime endDate, WorkItem workItem)
        {
            using (_driver)
            {
                Login(userName, password);

                Thread.Sleep(7000);

                if (endDate.Date <= startDate.Date)
                {
                    // Create today 
                    _driver.FindElement(By.XPath(" .//*[@id='CreateStart-button']")).Click();
                    Thread.Sleep(4000);

                    FillInDay(workItem);
                }
                else
                {
                    // create for range
                    while (startDate.Date <= endDate.Date)
                    {
                        _driver.FindElement(By.XPath(" .//*[@id='Create-button']")).Click();
                        Thread.Sleep(4000);
                        _driver.FindElement(By.XPath(" .//*[@id='dtp_TimeRecordingEntries_DocumentDate']"))
                            .SendKeys(startDate.Date.ToString("MM/dd/yyyy"));
                        _driver.FindElement(By.XPath(" .//*[@id='SaveCreate-button']")).Click();
                        Thread.Sleep(4000);

                        FillInDay(workItem);

                        startDate = startDate.AddDays(1);
                    }
                }

                Logout();
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

        private void Logout()
        {
            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='scrollContainer']/ul/li[9]/a")).Click();
            _driver.Quit();
        }

        private void Login(string userName, string password)
        {
            _driver.Navigate().GoToUrl(WebPortalUrl);

            GlobaWait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20));
            // Log in to Web Portal
            Thread.Sleep(7000);
            _driver.FindElement(By.XPath(".//*[@id='Username']/div/input")).SendKeys(userName);
            _driver.FindElement(By.XPath(".//*[@id='Password']/div/input")).SendKeys(password);
            Thread.Sleep(2000);
            _driver.FindElement(By.XPath(".//*[@id='Password']/div/input")).SendKeys(Keys.Enter);
        }
    }
}