using System.IO;

namespace Zapp.Transform
{
    /// <summary>
    /// Represents an interface used to transform configurations.
    /// </summary>
    public interface ITransformConfig
    {
        /// <summary>
        /// Transforms the given input config stream.
        /// </summary>
        /// <param name="input">Stream of the config to transform</param>
        /// <param name="output">Stream of the transformed config.</param>
        void Transform(Stream input, Stream output);
    }
}
