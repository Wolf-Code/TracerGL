using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.CollisionObjects;

namespace TracerRenderer.Data
{
    public class Mesh
    {
        private List<CollisionObject> colliders { set; get; }

        public IReadOnlyCollection<CollisionObject> Colliders => colliders;

        public Material Material { set; get; }

        public Model Parent { set; get; }

        public string Name { set; get; }

        private bool setData = false;

        private readonly int vertexBuffer;
        private readonly int indicesBuffer;
        private int elementCount;
        private const int vertexAttribs = 3;

        public Shader Shader { set; get; }

        public Transform Transform { private set; get; }

        public Mesh( Model parent )
        {
            parent.AddMesh( this );
            this.Parent = parent;

            Transform = new Transform { Parent = parent.Transform };
            Material = new Material( );

            colliders = new List<CollisionObject>( );

            vertexBuffer = GL.GenBuffer( );
            indicesBuffer = GL.GenBuffer( );

            Shader = Shader.Default;
        }

        public void SetTrianglesWithoutCollider( Vertex[ ] vertices, uint[ ] faces )
        {
            GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );
            GL.BufferData( BufferTarget.ArrayBuffer, new IntPtr( Vertex.SizeInBytes * vertices.Length ), vertices, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ArrayBuffer, 0 );

            GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesBuffer );
            GL.BufferData( BufferTarget.ElementArrayBuffer, new IntPtr( sizeof( uint ) * faces.Length ), faces, BufferUsageHint.StaticDraw );
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, 0 );

            elementCount = faces.Length;
            setData = true;
        }

        public void SetTrianglesWithoutCollider( Vertex[ ] vertices, Face[ ] faces )
        {
            List<uint> indexList = new List<uint>( );
            foreach ( Face face in faces )
                indexList.AddRange( face.Vertices );

            uint[ ] indexArray = indexList.ToArray( );

            SetTrianglesWithoutCollider( vertices, indexArray );
        }

        public void SetTrianglesWithCollider( Vertex[ ] vertices, Face[ ] faces )
        {
            SetTrianglesWithoutCollider( vertices, faces );
            colliders.Clear( );

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
                }
            }
        }

        public void Render( )
        {
            if ( !setData )
                throw new Exception( "Cannot render a mesh which has no data." );

            Shader.Use( );

            for ( int x = 0; x < vertexAttribs; x++ )
                GL.EnableVertexAttribArray( x );
            {
                // Vertex Array Buffer
                GL.BindBuffer( BufferTarget.ArrayBuffer, vertexBuffer );

                GL.VertexAttribPointer( 2, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, IntPtr.Zero );
                GL.VertexAttribPointer( 1, 2, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, ( IntPtr )( 2 * sizeof( float ) ) );
                GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, Vertex.SizeInBytes, ( IntPtr )( 5 * sizeof( float ) ) );

                Shader.BindAttributeLocation( "texcoords", 2 );
                Shader.BindAttributeLocation( "normal", 1 );
                Shader.BindAttributeLocation( "position", 0 );

                Shader.SetColor4( "diffuse", this.Material.Diffuse );

                // Index Array Buffer
                GL.BindBuffer( BufferTarget.ElementArrayBuffer, indicesBuffer );

                // Draw the elements in the element array buffer
                // Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
                GL.DrawElements( BeginMode.Triangles, elementCount, DrawElementsType.UnsignedInt, IntPtr.Zero );
            }
            for ( int x = 0; x < vertexAttribs; x++ )
                GL.DisableVertexAttribArray( x );
        }

        public void AddCollider( CollisionObject collider )
        {
            this.colliders.Add( collider );
        }
    }
}