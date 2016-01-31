using System.Collections;
using System.IO;

namespace GitPusher
{
    public class DirectoryWatcher
    {
        private readonly ChangesHarvester _changesHarvester;
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        public DirectoryWatcher(string basePath)
        {
            _changesHarvester = new ChangesHarvester(basePath);
            _watcher.Path = basePath;
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.LastAccess | NotifyFilters.FileName;
            _watcher.Filter = "*.*";
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Renamed += OnChanged;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _changesHarvester.AddChange(e.FullPath, e.ChangeType);
        }
    }
}