using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BotClean.ViewModel
{
    public class FileManager : IFileManager
    {
        private const string SaveFolder = @"C:\Quest Thread Cleaner";
        private const string SaveFile = @"BanList.txt";
        public string SavePath = Path.Combine(SaveFolder, SaveFile);

        public void SaveToFile(List<string> banList)
        {
            if (banList.Count < 1) return;

            var saveFolder = new DirectoryInfo(SaveFolder);

            try
            {
                if (!Directory.Exists(SaveFolder))
                {
                    saveFolder.Create();
                }

                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                }

                var saveString = banList.FirstOrDefault();
                for (var i = 1; i < banList.Count; i++)
                {
                    saveString = $"{saveString},{banList[i]}";
                }

                File.WriteAllText(SavePath, saveString);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }

        }

        public string LoadFromFile()
        {
            return File.Exists(SavePath) ? File.ReadAllText(SavePath) : string.Empty;
        }
    }
}