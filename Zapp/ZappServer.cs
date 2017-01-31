using Zapp.Rest;

namespace Zapp
{
    /// <summary>
    /// Represents a class which controls all the services.
    /// </summary>
    public class ZappServer : IZappServer
    {
        private readonly IRestService apiService;

        /// <summary>
        /// Initializes a new <see cref="ZappServer"/> with the specified dependencies.
        /// </summary>
        /// <param name="apiService">The instance of <see cref="IRestService"/> used for external actions.</param>
        public ZappServer(IRestService apiService)
        {
            this.apiService = apiService;
        }

        /// <summary>
        /// Starts the current instance of <see cref="ZappServer"/> and it's dependencies.
        /// </summary>
        public void Start()
        {
            apiService.Start();
        }
    }
}
