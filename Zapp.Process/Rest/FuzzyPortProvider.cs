using System.Net;
using System.Net.Sockets;

namespace Zapp.Process.Rest
{
    /// <summary>
    /// Represents an implementation of <see cref="IPortProvider"/> used with fuzzy-unsafe code.
    /// </summary>
    public class FuzzyPortProvider : IPortProvider
    {
        /// <summary>
        /// Scans for an available port to bind on.
        /// </summary>
        /// <inheritdoc />
        public int FindBindablePort()
        {
            using (var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                sock.Bind(new IPEndPoint(IPAddress.Loopback, 0));

                return (sock.LocalEndPoint as IPEndPoint).Port;
            }
        }
    }
}
