using System.IO;

namespace Zapp.Fuse
{
    /// <summary>
    /// Represents an interface to create instances of <see cref="IFusion"/>.
    /// </summary>
    public interface IFusionFactory
    {
        /// <summary>
        /// Creates a new <see cref="IFusion"/>.
        /// </summary>
        /// <param name="contentStream">Stream where the fusion is written to.</param>
        IFusion CreateNew(Stream contentStream);
    }
}
