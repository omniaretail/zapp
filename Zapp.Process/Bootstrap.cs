using Ninject;
using System;
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
            var zappProcess = default(IZappProcess);
            var processController = default(IProcessController);

            try
            {
                using (var kernel = new StandardKernel(new ZappProcessModule()))
                {
                    zappProcess = kernel.Get<IZappProcess>();
                    zappProcess.Start();

                    processController = kernel.Get<IProcessController>();
                    processController.WaitForCompletion();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
            }
            finally
            {
                (zappProcess as IDisposable)?.Dispose();
                zappProcess = null;

                (processController as IDisposable)?.Dispose();
                processController = null;
            }
        }
    }
}
