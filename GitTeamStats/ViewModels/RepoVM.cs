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
        private string _repoText = "";
        public string RepoText { get { if (string.IsNullOrEmpty(_repoText)) return "No Repository Selected"; else return _repoText; } set { _repoText = value; NotifyPropertyChanged(); } }
        public ICommand OpenRepoCommand { get; set; }
        private double _numRepoCommits;
        public double NumRepoCommits { get { return _numRepoCommits; } set { _numRepoCommits = value; NotifyPropertyChanged(); } }
        private ObservableCollection<Contributor> _contributors;
        public ObservableCollection<Contributor> Contributors { get { return _contributors; } set { _contributors = value; NotifyPropertyChanged(); } }

        // TODO: Refactor Contributor-Stats into its own MVVM Binding (ContributorVM, StatView) so duplicate code can be gotten rid of
        private Contributor _selectedContributor1;
        public Contributor SelectedContributor1 { get { return _selectedContributor1; } set { _selectedContributor1 = value; UpdateContributor1Stats(); NotifyPropertyChanged(); } }
        private double _percentCommits1;
        public double PercentCommits1 { get { return _percentCommits1; } set { _percentCommits1 = value; NotifyPropertyChanged(); } }
        private int _linesAdded1;
        public int LinesAdded1 { get { return _linesAdded1; } set { _linesAdded1 = value; NotifyPropertyChanged(); } }
        private int _linesDeleted1;
        public int LinesDeleted1 { get { return _linesDeleted1; } set { _linesDeleted1 = value; NotifyPropertyChanged(); } }
        private Dictionary<string, int> _edittedFiles1 { get; set; }
        public Dictionary<string, int> EdittedFiles1 { get { return _edittedFiles1; } set { _edittedFiles1 = value; NotifyPropertyChanged(); } }
        private EdittedFiles _selectedFile1 { get; set; }
        public EdittedFiles SelectedFile1 { get { return _selectedFile1; } set { _selectedFile1 = value; NotifyPropertyChanged(); } }

        private Contributor _selectedContributor2;
        public Contributor SelectedContributor2 { get { return _selectedContributor2; } set { _selectedContributor2 = value; UpdateContributor2Stats(); NotifyPropertyChanged(); } }
        private double _percentCommits2;
        public double PercentCommits2 { get { return _percentCommits2; } set { _percentCommits2 = value; NotifyPropertyChanged(); } }
        private int _linesAdded2;
        public int LinesAdded2 { get { return _linesAdded2; } set { _linesAdded2 = value; NotifyPropertyChanged(); } }
        private int _linesDeleted2;
        public int LinesDeleted2 { get { return _linesDeleted2; } set { _linesDeleted2 = value; NotifyPropertyChanged(); } }
        private Dictionary<string, int> _edittedFiles2 { get; set; }
        public Dictionary<string, int> EdittedFiles2 { get { return _edittedFiles2; } set { _edittedFiles2 = value; NotifyPropertyChanged(); } }
        private EdittedFiles _selectedFile2 { get; set; }
        public EdittedFiles SelectedFile2 { get { return _selectedFile2; } set { _selectedFile2 = value; NotifyPropertyChanged(); } }

        public RepoVM()
        {
            OpenRepoCommand = new DelegateCommand(OpenRepoCommandExecute);
            Contributors = new ObservableCollection<Contributor>();
            _edittedFiles1 = new Dictionary<string, int>();
            EdittedFiles1 = new Dictionary<string, int>();
            _edittedFiles2 = new Dictionary<string, int>();
            EdittedFiles2 = new Dictionary<string, int>();
        }

        private void UpdateContributor1Stats()
        {
            if (SelectedContributor1 != null)
            {
                for (int i = 0; i < SelectedContributor1.commits.Count; i++)
                {
                    Tree t1 = SelectedContributor1.commits[i].Tree;
                    if (SelectedContributor1.commits[i].Parents.Count() > 0)
                    {
                        Tree t2 = SelectedContributor1.commits[i].Parents.First().Tree;

                        var patch = Repository.Diff.Compare<Patch>(t2, t1);
                        LinesAdded1 = patch.LinesAdded;
                        LinesDeleted1 = patch.LinesDeleted;

                        EdittedFiles1 = new Dictionary<string, int>();
                        foreach (var ptch in patch)
                        {
                            if (EdittedFiles1.ContainsKey(ptch.Path))
                            {
                                EdittedFiles1[ptch.Path]++;
                            }
                            else
                            {
                                EdittedFiles1.Add(ptch.Path, 1);
                            }
                        }
                    }
                    else
                    {
                        LinesAdded1 = 0;
                        LinesDeleted1 = 0;
                        EdittedFiles1 = new Dictionary<string, int>();
                    }
                }

                PercentCommits1 = (double)SelectedContributor1.numberOfCommits / (double)Repository.Commits.Count();
            }
            else
            {
                LinesAdded1 = 0;
                LinesDeleted1 = 0;
                PercentCommits1 = 0;
                EdittedFiles1 = new Dictionary<string, int>();
            }
        }

        private void UpdateContributor2Stats()
        {
            if (SelectedContributor2 != null)
            {
                for (int i = 0; i < SelectedContributor2.commits.Count; i++)
                {
                    Tree t1 = SelectedContributor2.commits[i].Tree;
                    if (SelectedContributor2.commits[i].Parents.Count() > 0)
                    {
                        Tree t2 = SelectedContributor2.commits[i].Parents.First().Tree;

                        var patch = Repository.Diff.Compare<Patch>(t2, t1);
                        LinesAdded2 = patch.LinesAdded;
                        LinesDeleted2 = patch.LinesDeleted;

                        EdittedFiles2 = new Dictionary<string, int>();
                        foreach (var ptch in patch)
                        {
                            if (EdittedFiles2.ContainsKey(ptch.Path))
                            {
                                EdittedFiles2[ptch.Path]++;
                            }
                            else
                            {
                                EdittedFiles2.Add(ptch.Path, 1);
                            }
                        }
                    }
                    else
                    {
                        LinesAdded2 = 0;
                        LinesDeleted2 = 0;
                        EdittedFiles2 = new Dictionary<string, int>();
                    }
                }

                PercentCommits2 = (double)SelectedContributor2.numberOfCommits / (double)Repository.Commits.Count();
            }
            else
            {
                LinesAdded2 = 0;
                LinesDeleted2 = 0;
                PercentCommits2 = 0;
                EdittedFiles2 = new Dictionary<string, int>();
            }
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
