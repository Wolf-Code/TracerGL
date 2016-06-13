using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.Data;

namespace TracerRenderer
{
    public class Model
    {
        private readonly int vertexBuffer;
        private readonly int indicesBuffer;
        private readonly int elementCount;
        private const int vertexAttribs = 3;

        public Shader Shader { set; get; }

        public Transform Transform { private set; get; }

        public Matrix4 GetMatrix( )
        {
            Vector3 axis;
            float ang;
            Transform.Rotation.Quaternion.ToAxisAngle( out axis, out ang );

            return Matrix4.CreateFromAxisAngle( axis, ang ) * Matrix4.CreateTranslation( Transform.Position );
        }

        public Model( Vertex[ ] vertices, Face[ ] faces )
        {
            Transform = new Transform( );

            List<uint> indexList = new List<uint>( );
            foreach ( Face face in faces )
                indexList.AddRange( face.Vertices );

            uint[ ] indexArray = indexList.ToArray( );

            vertexBuffer = GL.GenBuffer( );
            GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );
            GL.BufferData( BufferTarget.ArrayBuffer, new IntPtr( Vertex.SizeInBytes * vertices.Length ), vertices, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

            indicesBuffer = GL.GenBuffer( );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesBuffer );
            GL.BufferData( BufferTarget.ElementArrayBuffer, new IntPtr( sizeof( uint ) * indexArray.Length ), indexArray, BufferUsageHint.StaticDraw  );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

            elementCount = indexArray.Length;
            Shader = Shader.Default;
        }

        public void Render( )
        {
            Shader.Use( );

            for ( int x = 0; x < vertexAttribs; x++ )
                GL.EnableVertexAttribArray( x );
            {
                // Vertex Array Buffer
                GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );
                GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, ( IntPtr ) ( 5 * sizeof ( float ) ) );

                // Index Array Buffer
                GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesBuffer );

                // Draw the elements in the element array buffer
                // Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
                GL.DrawElements( BeginMode.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero );
            }
            for ( int x = 0; x < vertexAttribs; x++ )
                GL.DisableVertexAttribArray( 0 );
        }
    }
}
