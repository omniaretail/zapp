using StackExchange.Redis;

namespace Zapp.Sync
{
    /// <summary>
    /// Represents a factory for the <see cref="IConnectionMultiplexer"/> instance.
    /// </summary>
    public interface IConnectionMultiplexerFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IConnectionMultiplexer"/> with the required <paramref name="connectionString"/>.
        /// </summary>
        /// <param name="connectionString">String that contains the connection info.</param>
        IConnectionMultiplexer CreateNew(string connectionString);
    }
}
