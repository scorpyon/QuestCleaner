using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using BotClean.Commands;

namespace BotClean.ViewModel
{
    public sealed class MainViewModel : ViewModelBase
    {
        public List<string> BanList { get; set; }
        private ICommand _cleanThreadCommand;
        private ICommand _banListCommand;
        public ObservableCollection<IPost> Posts { get; set; }

        public IFileManager FileManager => new FileManager();
        public ICommand CleanThreadCommand => _cleanThreadCommand ?? (_cleanThreadCommand = new CleanThreadCommand(BanList, FileManager, Posts));
        public ICommand BanListCommand => _banListCommand ?? (_banListCommand = new BanListCommand(BanList, FileManager));

        public MainViewModel()
        {
            BanList = new List<string>();
            Posts = new ObservableCollection<IPost>();
        }
    }
}