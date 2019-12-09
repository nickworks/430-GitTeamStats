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
        private Contributor _selectedContributor1;
        public Contributor SelectedContributor1 { get { return _selectedContributor1; } set { _selectedContributor1 = value; UpdateContributorStats(SelectedContributor1); NotifyPropertyChanged(); } }
        private Contributor _selectedContributor2;
        public Contributor SelectedContributor2 { get { return _selectedContributor2; } set { _selectedContributor2 = value; UpdateContributorStats(SelectedContributor2); NotifyPropertyChanged(); } }
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
        private Dictionary<string, int> _edittedFiles { get; set; }
        public Dictionary<string, int> EdittedFiles { get { return _edittedFiles; } set { _edittedFiles = value; NotifyPropertyChanged(); } }
        private EdittedFiles _selectedFile { get; set; }
        public EdittedFiles SelectedFile { get { return _selectedFile; } set { _selectedFile = value; NotifyPropertyChanged(); } }

        public RepoVM()
        {
            OpenRepoCommand = new DelegateCommand(OpenRepoCommandExecute);
            Contributors = new ObservableCollection<Contributor>();
            _edittedFiles = new Dictionary<string, int>();
            EdittedFiles = new Dictionary<string, int>();
        }

        private void UpdateContributorStats(Contributor contributor)
        {
            EdittedFiles.Clear();
            for (int i = 0; i < contributor.commits.Count; i++)
            {
                Tree t1 = contributor.commits[i].Tree;
                if (contributor.commits[i].Parents.Count() > 0)
                {
                    Tree t2 = contributor.commits[i].Parents.First().Tree;

                    var patch = Repository.Diff.Compare<Patch>(t2, t1);
                    LinesAdded = patch.LinesAdded;
                    LinesDeleted = patch.LinesDeleted;

                    foreach (var ptch in patch)
                    {
                        if (EdittedFiles.ContainsKey(ptch.Path))
                        {
                            EdittedFiles[ptch.Path]++;
                        }
                        else
                        {
                            EdittedFiles.Add(ptch.Path, 1);
                        }
                    }
                }
                else
                {
                    LinesAdded = 0;
                    LinesDeleted = 0;
                }
            }

            PercentCommits = (double)contributor.numberOfCommits / (double)Repository.Commits.Count();
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
                SelectedContributor1 = null;
                SelectedContributor2 = null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
