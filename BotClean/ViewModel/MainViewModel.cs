using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using BotClean.Commands;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BotClean.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public List<string> BanList { get; set; }
        private ICommand _cleanThreadCommand;
        private ICommand _banListCommand;
        private ICommand _launchBrowser;
        private ChromeDriver _driver;

        public IFileManager FileManager => new FileManager();
        public ICommand CleanThreadCommand => _cleanThreadCommand ?? (_cleanThreadCommand = new CleanThreadCommand(BanList, FileManager, _driver));
        public ICommand BanListCommand => _banListCommand ?? (_banListCommand = new BanListCommand(BanList, FileManager, _driver));

        public MainViewModel()
        {
            BanList = new List<string>();

            Process[] chromeDriverProcesses = Process.GetProcessesByName("chrome");
            foreach (var chromeDriverProcess in chromeDriverProcesses)
            {
                chromeDriverProcess.Kill();
            }

            var directoryInfo = Directory.GetParent(Environment.CurrentDirectory).Parent;
            if (directoryInfo == null) return;
            var driverDir = new DirectoryInfo(Path.Combine(directoryInfo.FullName, "ChromeDriver"));

            _driver = new ChromeDriver(driverDir.FullName) { Url = "https://www.hearthpwn.com/twitch-login?returnUrl=/" };

            var cookieAccept = _driver.FindElements(By.XPath("//*[text()='ACCEPT']"));
            cookieAccept.FirstOrDefault()?.Click();

            //LOGIN
            var loginButtonTwitch = _driver.FindElementByClassName("twitch-button");
            loginButtonTwitch?.Click();
        }
    }
}