using System.Collections.Generic;
using System.Web.Http;

namespace Zapp.Rest.Controllers
{
    public class SimpleController : ApiController
    {
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}
