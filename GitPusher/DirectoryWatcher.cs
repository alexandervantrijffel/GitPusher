using System;
using System.IO;
using Structura.SharedComponents.Utilities;

namespace GitPusher
{
    public class DirectoryWatcher
    {
        private readonly ChangesHarvester _changesHarvester;
        private readonly FileSystemWatcher _watcher = new FileSystemWatcher();

        public DirectoryWatcher(RepositoryConfigurationInfo config)
        {
            _changesHarvester = new ChangesHarvester(config);
            _watcher.Path = config.BaseDir;
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
            try
            {
                _changesHarvester.AddChange(e.FullPath, e.ChangeType);
            }
            catch (Exception ex)
            {
                FormatLoggerAccessor.Instance().Error(ex, 
                    $"Unhandled exception while processing file change event for file {e.FullPath}");
            }
        }
    }
}