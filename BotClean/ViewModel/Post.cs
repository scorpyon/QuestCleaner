namespace BotClean.ViewModel
{
    public class Post : IPost
    {
        public string Name { get; }
        public string NameLink { get; }
        public string PostText { get; }
        public string PostLink { get; }

        public Post(string name, string nameLink, string postText, string postLink)
        {
            Name = name;
            NameLink = nameLink;
            PostText = postText;
            PostLink = postLink;
        }
    }
}