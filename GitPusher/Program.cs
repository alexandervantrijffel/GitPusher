using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net;
using Structura.SharedComponents.Utilities;
using Topshelf;

namespace GitPusher
{
    class Program
    {
        static void Main(string[] args)
        {
            FormatLoggerAccessor.Initialize(() => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType));
            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;
            HostFactory.Run(x =>
            {
                x.Service<IService>(s =>
                {
                    
                    s.ConstructUsing(name => new GitService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });
                x.RunAsLocalSystem();
                x.UseLog4Net("log4net.config");
                x.SetDescription("Watch directories and commit and push changes to git automatically."); 
                x.SetDisplayName("GitPusher: Auto commit and push changes.");
                x.SetServiceName("GitPusher");
            });

        }

        private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            FormatLoggerAccessor.Instance().Error("Global exception handler error", (Exception)e.ExceptionObject);

            if (Debugger.IsAttached)
                Debugger.Break();
        }
    }
}
