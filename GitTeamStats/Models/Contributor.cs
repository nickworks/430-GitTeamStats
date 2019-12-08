using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;

namespace GitTeamStats.Models {
    /// <summary>
    /// This class represents a single contributor to a repo.
    /// Each contributor has a unique email address.
    /// </summary>
    public class Contributor {
        /// <summary>
        /// The username of the contributor.
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// The email address of the contributor.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// A list of all commits authored by the contributor.
        /// </summary>
        public List<Commit> commits = new List<Commit>();
        /// <summary>
        /// The total number of commits authored by the contributor.
        /// </summary>
        public int numberOfCommits { get { return commits.Count; } set { } }
        /// <summary>
        /// This constructor creates a contributor from a supplied commit.
        /// It grabs contributor info from the commit's Committer member.
        /// </summary>
        /// <param name="commit">A commit authored by the new Contributor</param>
        public Contributor(Commit commit) {
            name = commit.Committer.Name;
            email = commit.Committer.Email;
            commits.Add(commit);
        }
        /// <summary>
        /// This method will scan an entire repo and return a list of all contributors.
        /// </summary>
        /// <param name="repo">A specified repo</param>
        /// <returns>A list of all contributors</returns>
        static public List<Contributor> GetAll(Repository repo) {
            List<Contributor> contributors = new List<Contributor>();

            foreach (Commit commit in repo.Commits) {

                string user = commit.Committer.Email;

                Contributor contributor = contributors.Find(c => c.email == user);
                if (contributor != null) { // if contributor exists:
                    contributor.commits.Add(commit); // add commit to contributor's list of commits
                } else { // if contributor does NOT exist:
                    contributors.Add(new Contributor(commit)); // create contributor
                }
            }
            return contributors;
        }
    }
}
