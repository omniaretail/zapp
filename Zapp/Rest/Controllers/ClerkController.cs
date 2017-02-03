using System.Collections.Generic;
using System.Web.Http;

#pragma warning disable 1591

namespace Zapp.Rest.Controllers
{
    public class ClerkController : ApiController
    {
        [HttpGet]
        public IEnumerable<string> Deploy(string packageId, string deployVersion)
        {


            return new string[] { "ok", "then" };
        }
    }
}
