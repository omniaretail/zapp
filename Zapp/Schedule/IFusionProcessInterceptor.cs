namespace Zapp.Schedule
{
    /// <summary>
    /// Represents an interface used for intercepting events for <see cref="IFusionProcess"/>.
    /// </summary>
    public interface IFusionProcessInterceptor
    {
        /// <summary>
        /// Invoked when the process has started.
        /// </summary>
        /// <param name="fusionProcess">Process that has been started.</param>
        void OnStartupCalled(IFusionProcess fusionProcess);
    }
}
