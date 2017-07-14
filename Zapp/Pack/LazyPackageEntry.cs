using EnsureThat;
using System.Diagnostics;
using System.IO;

namespace Zapp.Pack
{
    /// <summary>
    /// Represents an implementation of <see cref="IPackageEntry"/> with lazy stream loading.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class LazyPackageEntry : IPackageEntry
    {
        private readonly LazyStream lazyStream;

        /// <summary>
        /// Represents the name of the entry.
        /// </summary>
        /// <inheritdoc />
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new <see cref="LazyPackageEntry"/>.
        /// </summary>
        /// <param name="name">Name of the entry.</param>
        /// <param name="lazyStream">Promise-style stream delegate.</param>
        public LazyPackageEntry(string name, LazyStream lazyStream)
        {
            EnsureArg.IsNotNullOrEmpty(name, nameof(name));
            EnsureArg.IsNotNull(lazyStream, nameof(lazyStream));

            Name = name;

            this.lazyStream = lazyStream;
        }

        /// <summary>
        /// Opens the entry with a streamed content.
        /// </summary>
        /// <inheritdoc />
        public Stream Open() => lazyStream();

        private string DebuggerDisplay => $"Entry: {Name}";
    }
}
