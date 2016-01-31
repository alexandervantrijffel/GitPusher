using System.Collections.Generic;

namespace GitPusher
{
    public class RepositoryConfigurationInfo
    {
        public const int DefaultWaitBeforeCommit = 120;

        public string BaseDir { get; set; }
        public int WaitBeforeCommit { get; set; }

        public IEnumerable<string> Remotes { get; set; }
    }
}