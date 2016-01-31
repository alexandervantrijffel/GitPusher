using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JsonConfig;

namespace GitPusher
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }

    public class GitService : IService
    {
        private IList<DirectoryWatcher> _directoryWatchers = new List<DirectoryWatcher>(); 

        public void Start()
        {
            try
            {
                ;

                var directories = LoadRepositoryConfig().ToList();
                Task.Factory.StartNew(() => 
                        Parallel.ForEach<string>(directories, directory =>
                        {
                            new GitCommitter().ProcessDirectory(directory);
                        }));
                foreach(var directory in directories)
                    _directoryWatchers.Add(new DirectoryWatcher(directory));
            }

            catch (Exception)
            {
                // todo log
                throw;
            }

            
        }

        private static IEnumerable<string> LoadRepositoryConfig()
        {
            Uri uri = new Uri(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)));
            dynamic config = Config.ApplyJsonFromPath(Path.Combine(uri.LocalPath, "configuration.json"));

            if (!(config.Repositories is Array) || config.Repositories.Length == 0)
            {
                throw new ConfigurationException("No Repositories found in file configuration.json");
            }

            for (int i = 0; i < config.Repositories.Length ; i++)
            {
                if (string.IsNullOrEmpty(config.Repositories[i].BaseDir))
                    throw new Exception("One or more Repositories in configuration.json have an invalid BaseDir property.");
                yield return config.Repositories[i].BaseDir;
            }
        }

        public void Stop()
        {
        }
    }
}