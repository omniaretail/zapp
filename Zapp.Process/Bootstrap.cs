using Ninject;
using System;
using System.IO;
using Zapp.Process.Controller;

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
            var processController = default(IProcessController);

            try
            {
                using (kernel = new StandardKernel(new ZappProcessModule()))
                {
                    zappProcess = kernel.Get<IZappProcess>();
                    zappProcess.Start();

                    processController = kernel.Get<IProcessController>();
                    processController.WaitForCompletion();
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                kernel?.Dispose();
                kernel = null;

                (zappProcess as IDisposable)?.Dispose();
                zappProcess = null;

                (processController as IDisposable)?.Dispose();
                processController = null;
            }
        }

        private static void HandleException(Exception ex)
        {
            var file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash_dump.log");

            File.WriteAllText(file, ex.ToString());
        }
    }
}
