using System.Linq;

namespace Zapp.Process
{
    public static class Bootstrap
    {
        public static void Main(string[] args)
        {
            var zappServerAddress = args?.FirstOrDefault();

            if (string.IsNullOrEmpty(zappServerAddress)) { }
        }
    }
}
