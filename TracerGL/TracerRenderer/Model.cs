using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.CollisionObjects;
using TracerRenderer.Data;

namespace TracerRenderer
{
    public class Model
    {
        public List<CollisionObject> CollisionObjects { get; } = new List<CollisionObject>( );

        private readonly int vertexBuffer;
        private readonly int indicesBuffer;
        private readonly int elementCount;
        private const int vertexAttribs = 3;

        public Shader Shader { set; get; }

        public Transform Transform { private set; get; }

        public Model( Vertex[ ] vertices, Face[ ] faces, bool createTrianglesCollisionObject = true )
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
            GL.BufferData( BufferTarget.ElementArrayBuffer, new IntPtr( sizeof ( uint ) * indexArray.Length ), indexArray, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

            elementCount = indexArray.Length;
            Shader = Shader.Default;

            if ( createTrianglesCollisionObject )
            {
                foreach ( Face f in faces )
                {
                    for ( int x = 0; x < f.Vertices.Length - 2; x++ )
                    {
                        Triangle t = new Triangle
                        {
                            V1 = vertices[ f.Vertices[ x ] ],
                            V2 = vertices[ f.Vertices[ x + 1 ] ],
                            V3 = vertices[ f.Vertices[ x + 2 ] ]
                        };

                        t.SetParent( this );

                        CollisionObjects.Add( t );
                    }
                }
            }
        }

        public Model( Vertex[ ] vertices, uint[ ] indexArray )
        {
            Transform = new Transform( );

            vertexBuffer = GL.GenBuffer( );
            GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );
            GL.BufferData( BufferTarget.ArrayBuffer, new IntPtr( Vertex.SizeInBytes * vertices.Length ), vertices, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

            indicesBuffer = GL.GenBuffer( );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesBuffer );
            GL.BufferData( BufferTarget.ElementArrayBuffer, new IntPtr( sizeof( uint ) * indexArray.Length ), indexArray, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

            elementCount = indexArray.Length;
            Shader = Shader.Default;
        }

        /// <summary>
        /// Adds a collision object to the model.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        public void AddCollisionObject( CollisionObject obj )
        {
            obj.SetParent( this );
            CollisionObjects.Add( obj );
        }

        public void Render( )
        {
            Shader.Use( );

            for ( int x = 0; x < vertexAttribs; x++ )
                GL.EnableVertexAttribArray( x );
            {
                // Vertex Array Buffer
                GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );

                GL.VertexAttribPointer( 2, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, IntPtr.Zero );
                GL.VertexAttribPointer( 1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, ( IntPtr ) ( 2 * sizeof ( float ) ) );
                GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, ( IntPtr ) ( 5 * sizeof ( float ) ) );

                Shader.BindAttributeLocation( "texcoords", 2 );
                Shader.BindAttributeLocation( "normal", 1 );
                Shader.BindAttributeLocation( "position", 0 );

                // Index Array Buffer
                GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesBuffer );

                // Draw the elements in the element array buffer
                // Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
                GL.DrawElements( BeginMode.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero );
            }
            for ( int x = 0; x < vertexAttribs; x++ )
                GL.DisableVertexAttribArray( x );
        }
    }
}
