using GitTeamStats.Models;
using LibGit2Sharp;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GitTeamStats.ViewModels
{
    class RepoVM
    {
        private string _repoText = "";
        public Repository Repository;
        public List<Contributor> Contributors;
        public Contributor Contributor;
        public ICommand OpenRepoCommand { get; set; }
        public string RepoText
        {
            get
            {
                if (string.IsNullOrEmpty(_repoText))
                    return "No Repository Selected";
                else
                    return _repoText;
            }
            set
            {
                _repoText = value;
            }
        }

        public RepoVM()
        {
            OpenRepoCommand = new DelegateCommand(OpenRepoCommandExecute);
        }

        private void OpenRepoCommandExecute()
        {
            Repository = RepoLoader.ShowDialog();

            if (Repository != null)
            {
                MessageBox.Show("Repo loaded");
                RepoText = Repository.Head.RemoteName;
                Contributors = Contributor.GetAll(Repository);
            }
            else
            {
                MessageBox.Show("Repo not loaded");
                RepoText = "";
            }
        }
    }
}
