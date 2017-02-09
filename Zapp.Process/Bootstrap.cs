using Ninject;
using System;
using System.IO;

namespace Zapp.Process
{
    /// <summary>
    /// Represents the main entry point of the zapp-process.
    /// </summary>
    public static class Bootstrap
    {
        /// <summary>
        /// Method that will be fired once the process starts
        /// </summary>
        /// <param name="args">Arguments provided by the spawner.</param>
        public static void Main(string[] args)
        {
            var kernel = default(IKernel);
            var zappProcess = default(IZappProcess);

            try
            {
                var settings = new NinjectSettings();
                settings.LoadExtensions = false;

                using (kernel = new StandardKernel(settings, new ZappProcessModule()))
                {
                    zappProcess = kernel.Get<IZappProcess>();
                    zappProcess.Start();
                }
            }
            catch (Exception ex)
            {
                var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_dump.log");

                File.WriteAllText(file, ex.ToString());
            }
            finally
            {
                kernel?.Dispose();
                kernel = null;

                (zappProcess as IDisposable)?.Dispose();
                zappProcess = null;
            }
        }
    }
}
