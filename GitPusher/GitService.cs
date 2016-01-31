using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitPusher
{
    public class GitService : IService
    {
        private IList<DirectoryWatcher> _directoryWatchers = new List<DirectoryWatcher>(); 

        public void Start()
        {
            try
            {
                var directories = new List<string>() { @"c:\data\testwatch"};
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

        public void Stop()
        {
        }
    }
}