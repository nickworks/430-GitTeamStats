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
        public Contributor SelectedContributor { get { return _selectedContributor; } set { _selectedContributor = value; UpdateContributorStats(); NotifyPropertyChanged(); } }
        private ObservableCollection<Contributor> _contributors;
        public ObservableCollection<Contributor> Contributors { get { return _contributors; } set { _contributors = value; NotifyPropertyChanged(); } }
        private string _repoText = "";
        public string RepoText { get { if (string.IsNullOrEmpty(_repoText)) return "No Repository Selected"; else return _repoText; } set { _repoText = value; NotifyPropertyChanged(); } }
        public ICommand OpenRepoCommand { get; set; }
        private double _numRepoCommits;
        public double NumRepoCommits { get { return _numRepoCommits; } set { _numRepoCommits = value; NotifyPropertyChanged(); } }
        private double _percentCommits;
        public double PercentCommits { get { return _percentCommits; } set { _percentCommits = value; NotifyPropertyChanged(); } }
        private int _linesAdded;
        public int LinesAdded { get { return _linesAdded; } set { _linesAdded = value; NotifyPropertyChanged(); } }
        private int _linesDeleted;
        public int LinesDeleted { get { return _linesDeleted; } set { _linesDeleted = value; NotifyPropertyChanged(); } }

        public RepoVM()
        {
            OpenRepoCommand = new DelegateCommand(OpenRepoCommandExecute);
            Contributors = new ObservableCollection<Contributor>();
        }

        private void UpdateContributorStats()
        {
            Tree commitTree = Repository.Head.Tip.Tree; // Main Tree
            Tree parentCommitTree = Repository.Head.Tip.Parents.First().Tree; // Secondary Tree

            var patch = Repository.Diff.Compare<TreeChanges>(parentCommitTree, commitTree); // Difference
            LinesDeleted = patch.Deleted.Count();
            Debug.Print(LinesDeleted.ToString());
            LinesAdded = patch.Added.Count();
            Debug.Print(LinesAdded.ToString());

            PercentCommits = (double)_selectedContributor.numberOfCommits / (double)Repository.Commits.Count();
        }

        private void OpenRepoCommandExecute()
        {
            Repository = RepoLoader.ShowDialog();

            if (Repository != null)
            {
                NumRepoCommits = Repository.Commits.Count();
                RepoText = Repository.Network.Remotes.First().Url;
                var contributors = Contributor.GetAll(Repository);
                Contributors.Clear();
                foreach (var item in contributors) { Contributors.Add(item); }
            }
            else
            {
                NumRepoCommits = 0;
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
