using GitTeamStats.Models;
using LibGit2Sharp;
using Microsoft.VisualStudio.PlatformUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GitTeamStats.ViewModels
{
    class RepoVM : INotifyPropertyChanged
    {
        public Repository Repository;
        private Contributor _selectedContributor;
        public Contributor SelectedContributor { get { return _selectedContributor; } set { _selectedContributor = value; NotifyPropertyChanged(); } }
        private ObservableCollection<Contributor> _contributors;
        public ObservableCollection<Contributor> Contributors { get { return _contributors; } set { _contributors = value; NotifyPropertyChanged(); } }
        private string _repoText = "";
        public string RepoText { get { if (string.IsNullOrEmpty(_repoText)) return "No Repository Selected"; else return _repoText; } set { _repoText = value; NotifyPropertyChanged(); } }
        public ICommand OpenRepoCommand { get; set; }

        public RepoVM()
        {
            OpenRepoCommand = new DelegateCommand(OpenRepoCommandExecute);
            Contributors = new ObservableCollection<Contributor>();
        }

        private void OpenRepoCommandExecute()
        {
            Repository = RepoLoader.ShowDialog();

            if (Repository != null)
            {
                RepoText = Repository.Network.Remotes.First().Url;
                var contributors = Contributor.GetAll(Repository);
                Contributors.Clear();
                foreach (var item in contributors) { Contributors.Add(item); }
            }
            else
            {
                RepoText = "";
                Contributors.Clear();
                SelectedContributor = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
