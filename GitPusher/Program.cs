using Topshelf;

namespace GitPusher
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<IService>(s =>
                {
                    s.ConstructUsing(name => new GitService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Watch directories and commit and push changes to git automatically."); 
                x.SetDisplayName("GitPusher: Auto commit and push changes.");
                x.SetServiceName("GitPusher");
            });

        }
    }
}
