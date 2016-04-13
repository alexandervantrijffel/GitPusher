using System.Linq;
using LibGit2Sharp;

namespace GitPusher
{
    public class GitCommitter
    {
        public void ProcessDirectory(RepositoryConfigurationInfo config)
        {
            if (!Repository.IsValid(config.BaseDir))
            {
                Repository.Init(config.BaseDir);
            }

            using (var repo = new Repository(config.BaseDir))
            {
                // todo, if user.name or user.email are not set in config, set them now!
                repo.Config.Set("diff.renames", "copies");

                var retrievalOptions = new StatusOptions {DetectRenamesInWorkDir = true, DetectRenamesInIndex = true};
                RepositoryStatus status = repo.RetrieveStatus(retrievalOptions);
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

                        foreach (var remoteName in config.Remotes)
                        {
                            var remote = repo.Network.Remotes[remoteName];
                            var options = new PushOptions();

                            //options.CredentialsProvider = new CredentialsHandler(
                            //(url, usernameFromUrl, types) =>
                            //    new UsernamePasswordCredentials()
                            //    {
                            //        Username = "myusername",
                            //        Password = "mypassword"
                            //    });

                            var pushRefSpec = @"refs/heads/master";
                            repo.Network.Push(remote, pushRefSpec, options, null, "GitPusher push");
                        }

                    }
                }
            }
        }
    }
}