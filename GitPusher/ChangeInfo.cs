using System.Collections.Generic;
using System.IO;
using CuttingEdge.Conditions;

namespace GitPusher
{
    public class ChangeInfo
    {
        public string FullPath { get; private set; }
        public string RelativePath { get; private set; }

        public IList<WatcherChangeTypes> ChangeTypes { get; set; } = new List<WatcherChangeTypes>();

        public ChangeInfo(string fullPath, string basePath, WatcherChangeTypes changeType)
        {
            RelativePath = fullPath.Replace(basePath, string.Empty);
            CuttingEdge.Conditions.Condition.Requires(RelativePath).IsNotEqualTo(fullPath,
                $"relative path '{RelativePath}' of file change should differ from fullpath '{FullPath}'");
            if (RelativePath.StartsWith(@"\"))
                RelativePath = RelativePath.Substring(1);
            FullPath = fullPath;
            ChangeTypes.Add(changeType);
        }
    }
}