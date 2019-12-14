using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using BotClean.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BotClean.Commands
{
    public class CleanThreadCommand : CommandBase
    {
        private readonly IFileManager _fileManager;
        private List<string> _banList;
        private ChromeDriver _driver;
        private ObservableCollection<IPost> _posts;

        public CleanThreadCommand(List<string> banList, IFileManager fileManager, ObservableCollection<IPost> posts)
        {
            _banList = banList ?? throw new ArgumentNullException(nameof(banList));
            _fileManager = fileManager ?? throw new ArgumentNullException(nameof(fileManager));
            _posts = posts ?? throw new ArgumentNullException(nameof(posts));
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
                    var posts = _driver.FindElementsByClassName("p-comment-post").ToList();
                    foreach (var post in posts)
                    {
                        var postNumberElement = post.FindElement(By.ClassName("j-comment-link"));
                        var postNumber = postNumberElement.Text.Remove(0, 1);
                        _driver.ExecuteScript("arguments[0].scrollIntoView();", postNumberElement);
                        var postTextElement = post.FindElement(By.ClassName("p-comment-wrapper"));
                        _driver.ExecuteScript("arguments[0].scrollIntoView();", postTextElement);
                        var postText = postTextElement.Text.ToLower();

                        foreach (var bannedUser in _banList)
                        {
                            if (!postText.Contains(bannedUser.ToLower())) continue;

                            var nameElement = post.FindElement(By.ClassName("p-comment-username"));
                            _driver.ExecuteScript("arguments[0].scrollIntoView();", nameElement);
                            var name = nameElement.Text;

                            var nameLink = $@"http://www.hearthpwn.com/members/{name}";
                            var postLink = $@"http://www.hearthpwn.com/forums/hearthstone-general/players-and-teams-discussion/214403-80g-quest-trading-play-a-friend-7?comment={postNumber}";

                            var newComment = new Post(name,nameLink, postText, postLink);
                            _posts.Add(newComment);
                        }
                    }

                    if (pages <= 1 || i == pages - 1) continue;
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
            _driver.Close();
            _driver.Dispose();
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