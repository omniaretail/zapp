using System.Net;
using System.Web.Http;
using System.Web.Http.Results;
using Zapp.Process.Libraries;

namespace Zapp.Process.Rest.Controllers
{
    /// <summary>
    /// Represents a controller used to control lifetime
    /// </summary>
    public class LifetimeController : ApiController
    {
        private readonly ILibraryService libraryService;

        /// <summary>
        /// Initializes a new <see cref="LifetimeController"/>.
        /// </summary>
        /// <param name="libraryService">Service used to control the loaded libraries.</param>
        public LifetimeController(
            ILibraryService libraryService)
        {
            this.libraryService = libraryService;
        }

        /// <summary>
        /// Used by the zapp-service to request start on the process.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/lifetime/startup")]
        public StatusCodeResult Startup()
        {
            libraryService.RunStartup();
            return StatusCode(HttpStatusCode.OK);
        }

        /// <summary>
        /// Used by the zapp-service to request teardown on the process.
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("api/lifetime/teardown")]
        public StatusCodeResult Teardown()
        {
            libraryService.RunTeardown();
            return StatusCode(HttpStatusCode.OK);
        }
    }
}
