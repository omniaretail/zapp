using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace Zapp.Core.NuGet
{
    /// <summary>
    /// Represents an basic implementation of <see cref="INuGetPackageResolver"/>.
    /// </summary>
    public class XmlNuGetPackageResolver : INuGetPackageResolver
    {
        private const string packageIdName = "id";
        private const string packageElementName = "package";

        /// <summary>
        /// Returns an collection with all the configured nuget-packages.
        /// </summary>
        /// <param name="assembly">Assembly to resolve the packages from.</param>
        /// <inheritdoc />
        public IEnumerable<string> GetPackageIds(Assembly assembly)
        {
            var resourceName = $"{assembly.GetName().Name}.packages.config";

            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = XmlReader.Create(resourceStream))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.Name == packageElementName)
                    {
                        yield return reader.GetAttribute(packageIdName);
                    }
                }
            }
        }
    }
}
