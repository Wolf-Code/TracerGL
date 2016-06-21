using System.Runtime.InteropServices;
using OpenTK;

namespace TracerRenderer.Data
{
    /// <summary>
    /// Vertex information.
    /// </summary>
    [StructLayout( LayoutKind.Sequential )]
    public struct Vertex
    { // mimic InterleavedArrayFormat.T2fN3fV3f
        /// <summary>
        /// The texture coordinates.
        /// </summary>
        public Vector2 TexCoord { set; get; }

        /// <summary>
        /// The normal.
        /// </summary>
        public Vector3 Normal { set; get; }

        /// <summary>
        /// The vertex's position.
        /// </summary>
        public Vector3 Position { set; get; }

        /// <summary>
        /// The size, in bytes, of a single vertex.
        /// </summary>
        public const byte SizeInBytes = 32;
    }
}
