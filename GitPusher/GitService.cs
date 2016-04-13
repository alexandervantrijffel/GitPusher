using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsonConfig;

namespace GitPusher
{
    public class GitService : IService
    {
        private IList<DirectoryWatcher> _directoryWatchers = new List<DirectoryWatcher>();

        public void Start()
        {
            var directories = LoadRepositoryConfig().ToList();
            Task.Factory.StartNew(() =>
                    Parallel.ForEach(directories, config =>
                    {
                        new GitCommitter().ProcessDirectory(config);
                    }));
            foreach (var directory in directories)
                _directoryWatchers.Add(new DirectoryWatcher(directory));
        }

        private static IEnumerable<RepositoryConfigurationInfo> LoadRepositoryConfig()
        {
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)));
            dynamic config = Config.ApplyJsonFromPath(Path.Combine(uri.LocalPath, "configuration.json"));

            if (!(config.Repositories is Array) || config.Repositories.Length == 0)
            {
                throw new ConfigurationException("No Repositories found in file configuration.json");
            }

            for (int i = 0; i < config.Repositories.Length; i++)
            {
                if (string.IsNullOrEmpty(config.Repositories[i].BaseDir))
                    throw new Exception("One or more Repositories in configuration.json have an invalid BaseDir property.");
                yield return new RepositoryConfigurationInfo
                {
                    BaseDir = config.Repositories[i].BaseDir,
                    WaitBeforeCommit = !(config.Repositories[i].WaitBeforeCommit is NullExceptionPreventer)
                        ? config.Repositories[i].WaitBeforeCommit
                        : RepositoryConfigurationInfo.DefaultWaitBeforeCommit,
                    Remotes = config.Repositories[i].Remotes is NullExceptionPreventer
                        ? Enumerable.Empty<string>()
                        : config.Repositories[i].Remotes
                };
            }
        }

        public void Stop()
        {
        }
    }
}