using Zapp.Rest;

namespace Zapp
{
    public class ZappServer : IZappServer
    {
        private readonly IRestService apiService;

        public ZappServer(IRestService apiService)
        {
            this.apiService = apiService;
        }

        public void Start()
        {
            apiService.Start();
        }
    }
}
