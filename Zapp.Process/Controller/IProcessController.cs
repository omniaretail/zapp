using System;

namespace Zapp.Process.Controller
{
    /// <summary>
    /// Represents an interface used to controll the lifecycle of the zapp-process.
    /// </summary>
    public interface IProcessController
    {
        /// <summary>
        /// Represents an bindable event which will invoke when the parent process is exited.
        /// </summary>
        event EventHandler ParentProcessExited;
    }
}
