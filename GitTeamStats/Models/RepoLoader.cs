using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LibGit2Sharp;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace GitTeamStats.Models {
    /// <summary>
    /// This class provides a few wrapper functions for opening repos.
    /// </summary>
    static public class RepoLoader {
        /// <summary>
        /// This method shows a dialog box that allows the user
        /// to navigate to a local repo.
        /// </summary>
        /// <returns>A repository. If failed or canceled, returns null.</returns>
        static public Repository ShowDialog() {
            CommonOpenFileDialog d = new CommonOpenFileDialog();
            d.IsFolderPicker = true;

            CommonFileDialogResult result = d.ShowDialog();
            switch (result) {
                case CommonFileDialogResult.Ok:
                    return OpenFromPath(d.FileName);
                case CommonFileDialogResult.None:
                    break;
                case CommonFileDialogResult.Cancel:
                    break;
            }
            
            return null;
        }
        /// <summary>
        /// This method attempts to open a repo from a supplied path.
        /// </summary>
        /// <param name="pathToRepo">The path to a repo folder</param>
        /// <returns>A repository. If failed, returns null.</returns>
        static public Repository OpenFromPath(string pathToRepo) {
            try {
                Repository repo = new Repository(pathToRepo);
                return repo;
            } catch (RepositoryNotFoundException e) {
                // TODO: handle error
                Console.WriteLine(e);
            }
            return null;
        }

    }
}
