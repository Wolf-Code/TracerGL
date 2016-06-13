using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TracerRenderer;
using TracerRenderer.Data;

namespace TracerGL
{
    class Window : GameWindow
    {
        private Model mdl, mdl2;
        private Camera cam;
        protected override void OnLoad( EventArgs e )
        {
            Vertex v1 = new Vertex { Position = new Vector3( -4, -1, 0 ) };
            Vertex v2 = new Vertex { Position = new Vector3( 1, -1, 0 ) };
            Vertex v3 = new Vertex { Position = new Vector3( 0, 1, 0 ) };

            Vertex[ ] vertices = { v1, v2, v3 };
            Face[ ] faces = { new Face { Vertices = new uint[ ]{ 0, 1, 2 } } };
            mdl = new Model( vertices, faces );
            mdl2 = new Model( vertices, faces );
            mdl2.Transform.Position=new Vector3(5,0,0);
            mdl2.Transform.Rotation=new Angle( 30, 30, 10 );
            GL.ClearColor( Color4.CornflowerBlue );

            cam = new Camera
            {
                Transform = new Transform( )
                {
                    Position = new Vector3( 0, 0, -10 ),
                    Rotation = new Angle( 0, 0, 0 )
                }
            };

            Mouse.Move += ( sender, args ) =>
            {
                if ( !Mouse[ MouseButton.Left ] ) return;

                cam.Transform.Rotation.Yaw += args.XDelta * 0.2f;
                cam.Transform.Rotation.Pitch -= args.YDelta * 0.2f;
            };

            base.OnLoad( e );
        }

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );

            // Set the viewport
            GL.Viewport( 0, 0, Width, Height );
            cam.SetAspect( Width / ( float ) Height );
        }

        protected override void OnUpdateFrame( FrameEventArgs e )
        {
            base.OnUpdateFrame( e );

            if ( Keyboard[ Key.W ] )
                cam.Transform.Position += cam.Transform.Forward * ( float ) e.Time * 10;

            if ( Keyboard[ Key.S ] )
                cam.Transform.Position -= cam.Transform.Forward * ( float ) e.Time * 10;

            if ( Keyboard[ Key.A ] )
                cam.Transform.Position -= cam.Transform.Right * ( float ) e.Time * 10;

            if ( Keyboard[ Key.D ] )
                cam.Transform.Position += cam.Transform.Right * ( float ) e.Time * 10;

            mdl2.Transform.Rotation.Roll += (float)e.Time * 25;
        }

        protected override void OnRenderFrame( FrameEventArgs e )
        {
            base.OnRenderFrame( e );

            // Clear the buffers
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            // Enable depth testing so our cube draws correctly
            GL.Enable( EnableCap.DepthTest );
            
            Matrix4 view = cam.GetMatrix( );
            Matrix4 VP = view * cam.Projection;
            Matrix4 MVP = mdl.Transform.GetMatrix(  ) * VP;

            mdl.Shader.BindAttributeLocation( "position", 0 );
            mdl.Shader.SetMatrix( "MVP", MVP );
            mdl.Render( );

            MVP = mdl2.Transform.GetMatrix(  ) * VP;
            mdl2.Shader.BindAttributeLocation( "position", 0 );
            mdl2.Shader.SetMatrix( "MVP", MVP );
            mdl2.Render( );

            SwapBuffers( );
        }
    }
}
