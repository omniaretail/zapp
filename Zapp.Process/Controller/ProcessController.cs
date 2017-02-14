using System;
using System.Threading;
using Zapp.Core;

using WinProcess = System.Diagnostics.Process;

namespace Zapp.Process.Controller
{
    /// <summary>
    /// Represents a implementation of <see cref="IProcessController"/> with simple logics.
    /// </summary>
    public class ProcessController : IProcessController, IDisposable
    {
        private ManualResetEvent resetEvent;

        /// <summary>
        /// Initializes a new <see cref="ProcessController"/>
        /// </summary>
        public ProcessController()
        {
            resetEvent = new ManualResetEvent(false);
        }

        /// <summary>
        /// Gets a variable which is defined for this process.
        /// </summary>
        /// <typeparam name="T">Type of the variable.</typeparam>
        /// <param name="key">Key of the variable.</param>
        /// <inheritdoc />
        public T GetVariable<T>(string key)
        {
            string value = Environment
                .GetEnvironmentVariable(key, EnvironmentVariableTarget.Process);

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Waits for completion of the process.
        /// </summary>
        /// <inheritdoc />
        public void WaitForCompletion()
        {
            var parentProcessId = GetVariable<int>(ZappVariables.ParentProcessIdEnvKey);

            var process = WinProcess
                .GetProcessById(parentProcessId);

            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => Cancel();

            resetEvent.WaitOne();
        }

        /// <summary>
        /// Cancels the current <see cref="WaitForCompletion"/>.
        /// </summary>
        /// <inheritdoc />
        public void Cancel() => resetEvent.Set();

        /// <summary>
        /// Release all used resources by the <see cref="ProcessController"/> instance.
        /// </summary>
        public void Dispose()
        {
            resetEvent?.Dispose();
            resetEvent = null;
        }
    }
}
