using GitTeamStats.Models;
using LibGit2Sharp;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GitTeamStats.ViewModels
{
    class RepoVM
    {
        public Repository repository;
        public ICommand OpenRepoCommand { get; set; }

        public RepoVM()
        {
            OpenRepoCommand = new DelegateCommand(OpenRepoCommandExecute);
        }

        private void OpenRepoCommandExecute()
        {
            repository = RepoLoader.ShowDialog();
        }
    }
}
