﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Framework
{
    public static class WebDriver
    {
        public static readonly TimeSpan ImplicitWaitTimeout = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan PageLoad = TimeSpan.FromSeconds(20);
        public static IWebDriver Instance { get; set; }

        private static ChromeOptions GetChromeOptions()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddUserProfilePreference("disable-popup-blocking", "true");
            return chromeOptions;
        }

        public static void Initialize()
        {
            Instance?.Quit();

            Instance = new ChromeDriver(GetChromeOptions());

            // waits
            Instance.Manage().Timeouts().PageLoad = PageLoad;
            Instance.Manage().Timeouts().ImplicitWait = ImplicitWaitTimeout;

            try
            {
                Instance.Manage().Window.Maximize();
            }

            catch
            {
                // ignore error
            }
        }

        public static void Cleanup()
        {
            Instance?.Quit();
        }

        public static void Wait(double maxWaitSec, Func<bool> expression)
        {
            while (maxWaitSec > 0 && !expression())
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(300));
                maxWaitSec = -0.3;


                Console.WriteLine($"Wait for it");
            }
        }

        public static void WaitForAjax()
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            while (st.ElapsedMilliseconds < 5000)
            {
                var ajaxIsComplete = (bool)(WebDriver.Instance as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    break;
                Thread.Sleep(100);
            }
        }

        public static string TakeScreenshot(string testName)
        {
            try
            {
                var fileName = Path.Combine($"{Path.GetTempPath()}", $"{testName}_{DateTime.UtcNow:yyyyMMMdd}.jpg");
                var screenShot = ((ITakesScreenshot)Instance).GetScreenshot();
                screenShot.SaveAsFile(fileName, ScreenshotImageFormat.Jpeg);
                return fileName;
            }
            catch (Exception e)
            {
                Log.Error($"Failed to take screenschot: {e}");
                return null;
            }
        }
    }
}
