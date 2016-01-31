using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LibGit2Sharp;

namespace GitPusher
{
    public class GitCommitter
    {
        public void ProcessDirectory(string repositoryPath)
        {
            if (!Repository.IsValid(repositoryPath))
            {
                Repository.Init(repositoryPath);
            }

            using (var repo = new Repository(repositoryPath))
            {
                repo.Config.Set("diff.renames", "copies");

                var options = new StatusOptions {DetectRenamesInWorkDir = true, DetectRenamesInIndex = true};
                RepositoryStatus status = repo.RetrieveStatus(options);
                if (status.IsDirty)
                {
                    bool toCommit = false;
                    foreach (var renamed in status.RenamedInWorkDir)
                    {
                        var oldFileName = renamed.IndexToWorkDirRenameDetails.OldFilePath;
                        repo.Stage(renamed.FilePath);
                        repo.Stage(oldFileName);
                        toCommit = true;
                    }
                    foreach (var untracked in status.Untracked)
                    {
                        repo.Stage(untracked.FilePath);
                        toCommit = true;
                    }
                    foreach (var modified in status.Modified)
                    {
                        repo.Stage(modified.FilePath);
                        toCommit = true;
                    }
                    foreach (var added in status.Added)
                    {
                        repo.Stage(added.FilePath);
                        toCommit = true;
                    }
                    foreach (var missing in status.Missing)
                    {
                        repo.Remove(missing.FilePath);
                        toCommit = true;
                    }

                    if (toCommit || status.Staged.Any())
                    {
                        repo.Commit("GitPusher commit.");
                    }

                    //Remote remote = repo.Network.Remotes["origin"];
                    //var options = new PushOptions();

                    //options.CredentialsProvider = new CredentialsHandler(
                    //(url, usernameFromUrl, types) =>
                    //    new UsernamePasswordCredentials()
                    //    {
                    //        Username = "myusername",
                    //        Password = "mypassword"
                    //    });

                    //var pushRefSpec = @"refs/heads/master";
                    //repo.Network.Push(remote, pushRefSpec, options, null, "push done...");
                }
            }
        }
    }
}