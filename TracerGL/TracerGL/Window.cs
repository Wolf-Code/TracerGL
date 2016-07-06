using System;
using System.IO;
using System.Linq;
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
        private Model quad;
        private Camera cam;
        private Shader textured;
        private OpenGLRenderer glRenderer;
        private PathTracingRenderer traceRenderer;
        private Renderer renderer;
        private World world;

        protected override void OnLoad( EventArgs e )
        {
            Vertex v1 = new Vertex { Position = new Vector3( -1f, -1, 0 ), TexCoord = new Vector2( 0, 0 ), Normal = Vector3.UnitY };
            Vertex v2 = new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ), Normal = Vector3.UnitY };
            Vertex v3 = new Vertex { Position = new Vector3( 0, 1, 0 ), TexCoord = new Vector2( 0, 1 ), Normal = Vector3.UnitY };

            Vertex[ ] vertices = { v1, v2, v3 };
            Face[ ] faces = { new Face { Vertices = new uint[ ] { 0, 1, 2 } } };

            ModelBuilder floorBuilder = new ModelBuilder( );
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
            Model floor = floorBuilder.GetModel( );
            floor.Meshes.First( ).Material.Diffuse = new Color( 0.5f, 0.5f, 0.5f );

            Model sphere = Util.CreateSphere( 0.4f, 16, 16 );
            sphere.Transform.Position = new Vector3( 0, 0.5f, 0 );
            sphere.Meshes.First( ).Material.Emission = new Color( 70, 70, 70 );

            Model sphere2 = Util.CreateSphere( 1f, 16, 16 );
            sphere2.Transform.Position = new Vector3( 2f, 1f, 0 );
            sphere2.Meshes.First().Material.Diffuse = new Color( 0.8f, 0.8f, 0.8f );

            Model sphere3 = Util.CreateSphere( 1f, 16, 16 );
            sphere3.Transform.Position = new Vector3( -2f, 1f, 0 );
            sphere3.Meshes.First( ).Material.Diffuse = new Color( 0.9f, 0.3f, 0.1f );
            sphere3.Meshes.First( ).Material.Type = Material.MaterialType.Reflective;

            Model sphere4 = Util.CreateSphere( 1f, 16, 16 );
            sphere4.Transform.Position = new Vector3( 0f, 3f, 0 );
            sphere4.Meshes.First( ).Material.Diffuse = new Color( 0.3f, 0.5f, 0.8f );

            world = new World( );
            world.AddModel( floor );
            world.AddModel( sphere );
            world.AddModel( sphere2 );
            world.AddModel( sphere3 );
            world.AddModel( sphere4 );

            GL.ClearColor( Color4.CornflowerBlue );

            cam = new Camera( Width, Height )
            {
                Transform = new Transform( new Vector3( 0, 0, 10 ) )
            };
            cam.SetFOV( 70 );

            Mouse.Move += ( sender, args ) =>
            {
                if ( !Mouse[ MouseButton.Left ] ) return;

                //cam.Transform.Rotation.Yaw += args.XDelta * 0.2f;
                cam.Transform.Rotation.AddRotation( -args.YDelta * 0.2f, -args.XDelta * 0.2f, 0 );
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

            quad = new Model( );
            Mesh quadMesh = new Mesh( quad );
            quadMesh.SetTrianglesWithCollider( new[ ]
            {
                new Vertex { Position = new Vector3( -1, -1, 0 ), TexCoord = new Vector2( 0, 0 ) },
                new Vertex { Position = new Vector3( -1, 1, 0 ), TexCoord = new Vector2( 0, 1 ) },
                new Vertex { Position = new Vector3( 1, 1, 0 ), TexCoord = new Vector2( 1, 1 ) },
                new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ) }
            }, new[ ] { new Face { Vertices = new uint[ ] { 0, 1, 2, 2, 3, 0 } } } );
            quadMesh.Shader = textured;

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

            cam.ResizeRenderTarget( Width, Height );
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
