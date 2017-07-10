using StackExchange.Redis;

namespace Zapp.Sync
{
    /// <summary>
    /// Represents a factory that creates <see cref="IConnectionMultiplexer"/> and connects directly.
    /// </summary>
    public class ConnectionMultiplexerFactory : IConnectionMultiplexerFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IConnectionMultiplexer"/> with the required <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">String that contains the connection info.</param>
        /// <inheritdoc />
        public IConnectionMultiplexer CreateNew(string connectionString) =>
            ConnectionMultiplexer.Connect(connectionString);
    }
}
