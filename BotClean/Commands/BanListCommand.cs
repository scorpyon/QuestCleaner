using System;
using System.Collections.Generic;
using BotClean.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BotClean.Commands
{
    public class BanListCommand : CommandBase
    {
        private readonly List<string> _banList;
        private IFileManager _fileManager;
        private ChromeDriver _driver;

        public BanListCommand(List<string> banList, IFileManager fileManager, ChromeDriver driver)
        {
            _banList = banList ?? throw new ArgumentNullException(nameof(banList));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public override void Execute(object parameter)
        {
            _driver.Navigate().GoToUrl($"https://www.hearthpwn.com/forums/hearthstone-general/players-and-teams-discussion/214403-80g-quest-trading-play-a-friend-7");
            PopulateBanList(_driver);

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