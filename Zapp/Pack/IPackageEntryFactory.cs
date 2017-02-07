namespace Zapp.Pack
{
    /// <summary>
    /// Represents an interface to create instances of <see cref="IPackageEntry"/>.
    /// </summary>
    public interface IPackageEntryFactory
    {
        /// <summary>
        /// Creates a new <see cref="IPackageEntry"/>.
        /// </summary>
        /// <param name="name">Name of the entry.</param>
        /// <param name="lazyStream">Promise-style stream delegate.</param>
        IPackageEntry CreateNew(string name, LazyStream lazyStream);
    }
}
