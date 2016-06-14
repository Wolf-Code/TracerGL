﻿using System;
using System.IO;
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
        private Model mdl, mdl2, mdl3, quad;
        private Camera cam;
        private Shader textured;
        private World world;

        protected override void OnLoad( EventArgs e )
        {
            GL.Enable( EnableCap.Texture2D );
            GL.Enable( EnableCap.DepthTest );

            Vertex v1 = new Vertex { Position = new Vector3( -4, -1, 0 ), TexCoord = new Vector2( 0, 0 ) };
            Vertex v2 = new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ) };
            Vertex v3 = new Vertex { Position = new Vector3( 0, 1, 0 ), TexCoord = new Vector2( 0, 1 ) };

            Vertex[ ] vertices = { v1, v2, v3 };
            Face[ ] faces = { new Face { Vertices = new uint[ ] { 0, 1, 2 } } };
            mdl = new Model( vertices, faces );
            mdl2 = new Model( vertices, faces )
            {
                Transform =
                {
                    Position = new Vector3( 5, 0, 0 ),
                    Rotation = new Angle( 30, 30, 10 )
                }
            };
            mdl3 = new Model( vertices, faces )
            {
                Transform =
                {
                    Position = new Vector3( -5, 0, 0 ),
                    Rotation = new Angle( 30, 30, 10 )
                }
            };


            world = new World( );
            world.AddModel( mdl );
            world.AddModel( mdl2 );
            world.AddModel( mdl3 );
            GL.ClearColor( Color4.CornflowerBlue );

            cam = new Camera( Width, Height )
            {
                Transform = new Transform
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
            }, new[ ] { new Face { Vertices = new uint[ ] { 0, 1, 2, 2, 3, 0 } } } );
            quad.Shader = textured;


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
                Transform = new Transform( )
                {
                    Position = cam.Transform.Position,
                    Rotation = cam.Transform.Rotation
                }
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
            
            // Get the view * projection matrix.
            Matrix4 view = cam.GetMatrix( );
            Matrix4 VP = view * cam.Projection;

            // We want to render to the framebuffer.
            cam.BindForRendering( );
            {
                GL.Clear( ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit );

                foreach ( Model model in world.Models )
                {
                    model.Shader.Use( );
                    model.Shader.BindAttributeLocation( "position", 0 );
                    model.Shader.SetMatrix( "MVP", model.Transform.GetMatrix( ) * VP );
                    model.Render( );
                }
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
