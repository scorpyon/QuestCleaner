using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BotClean.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BotClean.Commands
{
    public class BanListCommand : CommandBase
    {
        private readonly List<string> _banList;
        private readonly IFileManager _fileManager;
        private ChromeDriver _driver;

        public BanListCommand(List<string> banList, IFileManager fileManager)
        {
            _banList = banList ?? throw new ArgumentNullException(nameof(banList));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
        }

        public override void Execute(object parameter)
        {
            var directoryInfo = Directory.GetParent(Environment.CurrentDirectory).Parent;
            if (directoryInfo == null) return;
            var driverDir = new DirectoryInfo(Path.Combine(directoryInfo.FullName, "ChromeDriver"));

            _driver = new ChromeDriver(driverDir.FullName) { Url = "https://www.hearthpwn.com/" };

            var cookieAccept = _driver.FindElements(By.XPath("//*[text()='ACCEPT']"));
            cookieAccept.FirstOrDefault()?.Click();

            _driver.Navigate().GoToUrl($"https://www.hearthpwn.com/forums/hearthstone-general/players-and-teams-discussion/214403-80g-quest-trading-play-a-friend-7");

            PopulateBanList(_driver);

            _driver.Close();
            _driver.Dispose();

            _fileManager.SaveToFile(_banList);
        }

        private void PopulateBanList(ChromeDriver driver)
        {
            var firstListPost = driver.FindElementById("forum-post-body-2");
            
            driver.ExecuteScript("arguments[0].scrollIntoView();", firstListPost);
            var firstListTds = firstListPost.FindElements(By.XPath("//td"));

            foreach (var td in firstListTds)
            {
                driver.ExecuteScript("arguments[0].scrollIntoView();", td);
                if (string.IsNullOrEmpty(td.Text)) continue;
                if (td.Text != "-")
                {
                    var banName = td.Text.Split('#');
                    _banList.Add($"{banName[0]}{banName[1]}");
                    _banList.Add($"{banName[0]}#{banName[1]}");
                    _banList.Add($"{banName[0]} {banName[1]}");
                    _banList.Add($"{banName[0]} #{banName[1]}");
                    _banList.Add($"{banName[0]} # {banName[1]}");
                    _banList.Add($"{banName[0]}# {banName[1]}");
                }
            }
        }
    }
}