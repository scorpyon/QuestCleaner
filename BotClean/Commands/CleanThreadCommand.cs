using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using BotClean.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace BotClean.Commands
{
    public class CleanThreadCommand : CommandBase
    {
        private readonly IFileManager _fileManager;
        private List<string> _banList;
        private ChromeDriver _driver;

        public CleanThreadCommand(List<string> banList, IFileManager fileManager, ChromeDriver driver)
        {
            _banList = banList ?? throw new ArgumentNullException(nameof(banList));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        public override void Execute(object parameter)
        {
            //var usernameField = driver.FindElementById("username");
            //if(usernameField == null) return;
            //usernameField.SendKeys("dannyjaye");

            //var passwordDiv = driver.FindElementById("password");
            //var passwordBox = passwordDiv.FindElement(By.ClassName("text"));
            //if(passwordBox == null) return;
            //passwordBox.SendKeys("orange00");

            //var loginButton = driver.FindElementByClassName("js-login-button");
            //if(loginButton == null) return;
            //loginButton.Click();

            _driver.Navigate().GoToUrl($"https://www.hearthpwn.com/forums/hearthstone-general/players-and-teams-discussion/214403-80g-quest-trading-play-a-friend-7");

            var listingContainer = _driver.FindElementByClassName("listing-container");
            if(listingContainer == null) return;
            var paginationClass = listingContainer.FindElements(By.ClassName("b-pagination-item")).Where(x => x.TagName == "li").ToList();
            var lastPage = paginationClass[5].Text;

            _driver.Navigate().GoToUrl($"http://www.hearthpwn.com/forums/hearthstone-general/players-and-teams-discussion/214403-80g-quest-trading-play-a-friend-7?page={lastPage}");
        
            RetrieveBanListFromFile();
            try
            {
                var pages = int.Parse(parameter.ToString());

                for (var i = 0; i < pages; i++)
                {
                    var posts = _driver.FindElementsByClassName("p-comment-wrapper").ToList();
                    foreach (var post in posts)
                    {
                        _driver.ExecuteScript("arguments[0].scrollIntoView();", post);
                        var postText = post.Text.ToLower();
                        foreach (var name in _banList)
                        {
                            if (postText.Contains(name.ToLower()))
                            {
                                // Delete the post
                                var adminButton = post.FindElement(By.ClassName("p-comment-actionsAdmin"));
                                if (adminButton == null) continue;
                                adminButton.Click();
                                var deleteOption = adminButton.FindElement(By.Id("nav-delete"));
                                if (deleteOption == null) continue;
                                deleteOption.Click();
                                Thread.Sleep(500);
                                var buttons = _driver.FindElementsByTagName("button").ToList();
                                foreach (var webElement in buttons)
                                {
                                    if (webElement.Text == "Delete")
                                    {
                                        webElement.Click();
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (pages <= 1) continue;
                    // Next page
                    lastPage = (int.Parse(lastPage) - 1).ToString();
                    _driver.Navigate()
                        .GoToUrl($"http://www.hearthpwn.com/forums/hearthstone-general/players-and-teams-discussion/214403-80g-quest-trading-play-a-friend-7?page={lastPage}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RetrieveBanListFromFile()
        {
            var banListString = _fileManager.LoadFromFile();
            if (!string.IsNullOrEmpty(banListString))
            {
                _banList = banListString.Split(',').ToList();
            }
        }
    }
}