namespace GitPusher
{
    public class GitService : IService
    {
        private DirectoryWatcher _directoryWatcher; 

        public void Start()
        {
            _directoryWatcher = new DirectoryWatcher(@"c:\data\testwatch");
        }

        public void Stop()
        {
        }
    }
}