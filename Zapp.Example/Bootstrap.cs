using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using Ninject;
using StackExchange.Redis;
using System;
using System.Threading;
using Zapp.Fuse;
using Zapp.Pack;
using Zapp.Server;
using Zapp.Sync;

namespace Zapp.Example
{
    public static class Bootstrap
    {
        private static ManualResetEvent resetEvent;

        static Bootstrap()
        {
            resetEvent = new ManualResetEvent(false);
        }

        public static void Main(string[] args)
        {
            ConfigureLog();

            using (var kernel = new StandardKernel(new ZappModule()))
            {
                kernel.Bind<IFusionFilter>().To<FusionProcessRenameFilter>();
                
                var server = kernel.Get<IZappServer>();
                server.Start();

                ThreadPool.QueueUserWorkItem((state) => ListenConsoleInput());

                resetEvent.WaitOne();
            }
        }

        private static void ConfigureLog()
        {
            var appender = new ColoredConsoleAppender
            {
                Threshold = Level.All,
                Layout = new PatternLayout(
                    "%d{HH:mm:ss} %level [%thread] %logger => %message%newline"
                ),
            };
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Debug,
                ForeColor = ColoredConsoleAppender.Colors.Cyan
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Info,
                ForeColor = ColoredConsoleAppender.Colors.White
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Warn,
                ForeColor = ColoredConsoleAppender.Colors.Yellow
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Error,
                ForeColor = ColoredConsoleAppender.Colors.Red
                    | ColoredConsoleAppender.Colors.HighIntensity
            });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors
            {
                Level = Level.Fatal,
                ForeColor = ColoredConsoleAppender.Colors.White
                    | ColoredConsoleAppender.Colors.HighIntensity,
                BackColor = ColoredConsoleAppender.Colors.Red
            });

            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);
        }

        private static void ListenConsoleInput()
        {
            string userInput = null;

            while ((userInput = Console.ReadLine()) != null)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }

            resetEvent.Set();
        }
    }
}
