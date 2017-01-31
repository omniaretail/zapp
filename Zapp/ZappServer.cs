using Zapp.Packages;
using Zapp.Rest;

namespace Zapp
{
    /// <summary>
    /// Represents a class which controls all the services.
    /// </summary>
    public class ZappServer : IZappServer
    {
        private readonly IRestService apiService;
        private readonly IPackageService packageService;

        /// <summary>
        /// Initializes a new <see cref="ZappServer"/> with the specified dependencies.
        /// </summary>
        /// <param name="apiService">The instance of <see cref="IRestService"/> used for web actions.</param>
        /// <param name="packageService">The instance of <see cref="IPackageService"/> used for package actions.</param>
        public ZappServer(
            IRestService apiService,
            IPackageService packageService)
        {
            this.apiService = apiService;
            this.packageService = packageService;
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
