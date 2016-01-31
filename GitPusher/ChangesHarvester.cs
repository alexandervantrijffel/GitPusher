using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace GitPusher
{
    public class ChangesHarvester
    {
        private readonly RepositoryConfigurationInfo _config;
        private ConcurrentDictionary<string, ChangeInfo> _changes = new ConcurrentDictionary<string, ChangeInfo>();
        private Timer _timer;

        public ChangesHarvester(RepositoryConfigurationInfo config)
        {
            _config = config;
        }

        public void AddChange(string path, WatcherChangeTypes changeType)
        {
            var fileInfo = new ChangeInfo(path, _config.BaseDir, changeType);

            if (fileInfo.RelativePath.StartsWith(".git", StringComparison.InvariantCultureIgnoreCase))
                return;

            _changes.AddOrUpdate(fileInfo.RelativePath, fileInfo, (key, existingInfo) =>
            {
                if (!existingInfo.ChangeTypes.Any(c => c == changeType))
                    existingInfo.ChangeTypes.Add(changeType);
                return existingInfo;
            });

            if (_timer == null)
            {
                _timer = new Timer(OnTimer, _changes, TimeSpan.FromSeconds(_config.WaitBeforeCommit), Timeout.InfiniteTimeSpan);
            }
        }

        private void OnTimer(object state)
        {
            var dictionary = (ConcurrentDictionary<string, ChangeInfo>)state;
            // copy items to buffer
            var fileInfos = dictionary.Values.ToList();
            // and clear items to be processed
            dictionary.Clear();
            if (fileInfos.Any())
                new GitCommitter().ProcessDirectory(_config.BaseDir);
            _timer.Dispose();
            _timer = null;
        }
    }
}