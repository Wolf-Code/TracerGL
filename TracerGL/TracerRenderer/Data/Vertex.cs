using System.Runtime.InteropServices;
using OpenTK;

namespace TracerRenderer.Data
{
    [StructLayout( LayoutKind.Sequential )]
    public struct Vertex
    { // mimic InterleavedArrayFormat.T2fN3fV3f
        public Vector2 TexCoord { set; get; }
        public Vector3 Normal { set; get; }
        public Vector3 Position { set; get; }

        /// <summary>
        /// The size, in bytes, of a single vertex.
        /// </summary>
        public const byte SizeInBytes = 32;
    }
}
