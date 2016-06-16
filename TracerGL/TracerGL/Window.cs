using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using TracerRenderer;
using TracerRenderer.Data;
using TracerRenderer.Renderers;

namespace TracerGL
{
    class Window : GameWindow
    {
        private Model mdl, mdl2, mdl3, floor, quad;
        private Camera cam;
        private Shader textured;
        private OpenGLRenderer glRenderer;
        private PathTracingRenderer traceRenderer;
        private Renderer renderer;
        private World world;

        protected override void OnLoad( EventArgs e )
        {
            Vertex v1 = new Vertex { Position = new Vector3( -1f, -1, 0 ), TexCoord = new Vector2( 0, 0 ) };
            Vertex v2 = new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ) };
            Vertex v3 = new Vertex { Position = new Vector3( 0, 1, 0 ), TexCoord = new Vector2( 0, 1 ) };

            Vertex[ ] vertices = { v1, v2, v3 };
            Face[ ] faces = { new Face { Vertices = new uint[ ] { 0, 1, 2 } } };
            mdl = new Model( vertices, faces );
            mdl2 = new Model( vertices, faces )
            {
                Transform =
                {
                    Position = new Vector3( 1, 0, 0 )
                }
            };
            mdl3 = new Model( vertices, faces )
            {
                Transform =
                {
                    Position = new Vector3( -5, 0, 0 ),
                    Rotation = new Angle( 30, 30, 10 ),
                    Parent = mdl2.Transform
                }
            };

            ModelBuilder floorBuilder = new ModelBuilder(  );
            floorBuilder.AddVertex( new Vertex
            {
                Position = new Vector3( -100, 0, -100 ),
                TexCoord = new Vector2( 0, 0 ),
                Normal = Vector3.UnitY
            } );
            floorBuilder.AddVertex( new Vertex
            {
                Position = new Vector3( -100, 0, 100 ),
                TexCoord = new Vector2( 0, 1 ),
                Normal = Vector3.UnitY
            } );
            floorBuilder.AddVertex( new Vertex
            {
                Position = new Vector3( 100, 0, 100 ),
                TexCoord = new Vector2( 1, 1 ),
                Normal = Vector3.UnitY
            } );
            floorBuilder.AddVertex( new Vertex
            {
                Position = new Vector3( 100, 0, -100 ),
                TexCoord = new Vector2( 1, 0 ),
                Normal = Vector3.UnitY
            } );
            floorBuilder.AddFace( new Face { Vertices = new uint[ ] { 0, 1, 2 } } );
            floorBuilder.AddFace( new Face { Vertices = new uint[ ] { 2, 3, 0 } } );
            floor = floorBuilder.GetModel( );

            world = new World( );
            world.AddModel( mdl );
            world.AddModel( mdl2 );
            world.AddModel( mdl3 );
            world.AddModel( floor );
            GL.ClearColor( Color4.CornflowerBlue );

            cam = new Camera( Width, Height )
            {
                Transform = new Transform( new Vector3( 0, 0, -10 ) )
            };

            Mouse.Move += ( sender, args ) =>
            {
                if ( !Mouse[ MouseButton.Left ] ) return;

                //cam.Transform.Rotation.Yaw += args.XDelta * 0.2f;
                cam.Transform.Rotation.AddRotation( args.YDelta * 0.2f, args.XDelta * 0.2f );
                //cam.Transform.Rotation.Pitch -= args.YDelta * 0.2f;
            };

            Keyboard.KeyDown += ( sender, args ) =>
            {
                if ( args.Key == Key.Space )
                {
                    if ( renderer is OpenGLRenderer )
                        renderer = traceRenderer;
                    else
                        renderer = glRenderer;
                }
            };

            textured = new Shader( );
            textured.AddShader( File.ReadAllText( "Shaders/texturedQuad.frag" ), ShaderType.FragmentShader );
            textured.AddShader( File.ReadAllText( "Shaders/texturedQuad.vert" ), ShaderType.VertexShader );
            textured.Link( );

            quad = new Model( new[ ]
            {
                new Vertex { Position = new Vector3( -1, -1, 0 ), TexCoord = new Vector2( 0, 0 ) },
                new Vertex { Position = new Vector3( -1, 1, 0 ), TexCoord = new Vector2( 0, 1 ) },
                new Vertex { Position = new Vector3( 1, 1, 0 ), TexCoord = new Vector2( 1, 1 ) },
                new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ) }
            }, new[ ] { new Face { Vertices = new uint[ ] { 0, 1, 2, 2, 3, 0 } } } ) { Shader = textured };

            traceRenderer = new PathTracingRenderer( 200, 200 );
            glRenderer = new OpenGLRenderer( );

            renderer = glRenderer;

            base.OnLoad( e );
        }

        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );

            // Set the viewport
            GL.Viewport( 0, 0, Width, Height );
            cam.SetAspect( Width / ( float ) Height );

            cam = new Camera( Width, Height )
            {
                Transform = new Transform( cam.Transform.Position, cam.Transform.Rotation )
            };
        }

        protected override void OnUpdateFrame( FrameEventArgs e )
        {
            base.OnUpdateFrame( e );

            if ( Keyboard[ Key.W ] )
                cam.Transform.Position += cam.Transform.Forward * ( float ) e.Time * 10;

            if ( Keyboard[ Key.S ] )
                cam.Transform.Position -= cam.Transform.Forward * ( float ) e.Time * 10;

            if ( Keyboard[ Key.A ] )
                cam.Transform.Position += cam.Transform.Right * ( float ) e.Time * 10;

            if ( Keyboard[ Key.D ] )
                cam.Transform.Position -= cam.Transform.Right * ( float ) e.Time * 10;

            //mdl2.Transform.Rotation.Roll += (float)e.Time * 25;
            mdl2.Transform.Rotation.AddRotation( 0, 0, ( float ) e.Time * 25 );
        }

        protected override void OnRenderFrame( FrameEventArgs e )
        {
            base.OnRenderFrame( e );

            // Clear the buffers
            GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

            // Enable depth testing so our cube draws correctly
            GL.Enable( EnableCap.DepthTest );



            // We want to render to the framebuffer.
            cam.BindForRendering( );
            {
                GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );
                //GL.Begin( BeginMode.CollisionObjects );
                //GL.Vertex3( new Vector3( -0.3f, -1, 0 ) );
                //GL.Vertex3( new Vector3( 1, -1, 0 ) );
                //GL.Vertex3( new Vector3( 0, 1, 0 ) );
                //GL.End( );
                renderer.Render( cam, world );
            }
            // And we want to stop rendering to the framebuffer.
            cam.RenderTarget.Unbind( );


            // Render the fullscreen quad containing the framebuffer's texture.
            DrawFullscreenQuad( );

            SwapBuffers( );
        }

        private void DrawFullscreenQuad( )
        {
            textured.Use( );
            textured.SetTexture( "quadTexture", cam.RenderTarget.ColorTexture );

            quad.Render( );
        }
    }
}
