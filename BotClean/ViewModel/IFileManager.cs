using System.Collections.Generic;

namespace BotClean.ViewModel
{
    public interface IFileManager
    {
        void SaveToFile(List<string> banList);
        string LoadFromFile();
    }
}