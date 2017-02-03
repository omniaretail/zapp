using System;

namespace Zapp.Config
{
    /// <summary>
    /// Represents an interface for loading the <see cref="ZappConfig"/>.
    /// </summary>
    public interface IConfigStore
    {
        /// <summary>
        /// Represents the lazy for the zapp-config.
        /// </summary>
        Lazy<ZappConfig> Lazy { get; }
    }
}