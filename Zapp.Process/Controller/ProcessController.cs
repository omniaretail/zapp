using System;
using Zapp.Core;
using Zapp.Core.Clauses;
using WinProcess = System.Diagnostics.Process;

namespace Zapp.Process.Controller
{
    /// <summary>
    /// Represents a implementation of <see cref="IProcessController"/> with simple logics.
    /// </summary>
    public class ProcessController : IProcessController
    {
        /// <summary>
        /// Represents an bindable event which will invoke when the parent process is exited.
        /// </summary>
        /// <inheritdoc />
        public event EventHandler ParentProcessExited;

        /// <summary>
        /// Initializes a new <see cref="ProcessController"/>
        /// </summary>
        public ProcessController()
        {
            var rawParentId = Environment.GetEnvironmentVariable(
                ZappVariables.ParentProcessIdEnvKey,
                EnvironmentVariableTarget.Process
            );

            var process = GetParentProcess(rawParentId);
            process.EnableRaisingEvents = true;
            process.Exited += (s, e) => Emit();
        }

        private WinProcess GetParentProcess(string rawParentId)
        {
            Guard.ParamNotNullOrEmpty(rawParentId, nameof(rawParentId));

            return WinProcess.GetProcessById(Convert.ToInt32(rawParentId));
        }

        private void Emit() => ParentProcessExited?.Invoke(this, new EventArgs());
    }
}
