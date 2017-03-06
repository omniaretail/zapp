using System.Collections.Generic;
using System.Reflection;

namespace Zapp.Core.NuGet
{
    /// <summary>
    /// Represents an interface for reading package id's from the packages.config.
    /// </summary>
    public interface INuGetPackageResolver
    {
        /// <summary>
        /// Returns an collection with all the configured nuget-packages.
        /// </summary>
        /// <param name="assembly">Assembly to resolve the packages from.</param>
        IEnumerable<string> GetPackageIds(Assembly assembly);
    }
}
