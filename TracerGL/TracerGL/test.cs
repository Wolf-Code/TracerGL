using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace TracerGL
{
    class test : GameWindow
    {
        int vbo;

        void CreateVertexBuffer( )
        {
            Vector3[ ] vertices = new Vector3[ 3 ];
            vertices[ 0 ] = new Vector3( -0.1f, -1f, 0f );
            vertices[ 1 ] = new Vector3( 1f, -1f, 0f );
            vertices[ 2 ] = new Vector3( 0f, 1f, 0f );

            GL.GenBuffers( 1, out vbo );
            GL.BindBuffer( BufferTarget.ArrayBuffer, vbo );
            GL.BufferData<Vector3>( BufferTarget.ArrayBuffer,
                new IntPtr( vertices.Length * Vector3.SizeInBytes ),
                vertices, BufferUsageHint.StaticDraw );
        }

        protected override void OnLoad( EventArgs e )
        {
            GL.ClearColor( Color.Brown );
            CreateVertexBuffer( );
        }

        protected override void OnRenderFrame( FrameEventArgs e )
        {
            GL.Clear( ClearBufferMask.ColorBufferBit );

            GL.EnableVertexAttribArray( 0 );
            GL.BindBuffer( BufferTarget.ArrayBuffer, vbo );
            GL.VertexAttribPointer( 0, 3, VertexAttribPointerType.Float, false, 0, 0 );

            GL.DrawArrays( BeginMode.Triangles, 0, 3 );

            GL.DisableVertexAttribArray( 0 );

            SwapBuffers( );
        }

    }
}
