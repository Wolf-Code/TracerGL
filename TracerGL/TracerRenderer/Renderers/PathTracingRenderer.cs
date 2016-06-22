using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.Data;

namespace TracerRenderer.Renderers
{
    /// <summary>
    /// A renderer which renders an image using path tracing techniques.
    /// </summary>
    public class PathTracingRenderer : Renderer
    {
        private byte[ ] img = new byte[ 0 ];

        /// <summary>
        /// The width of the image to be pathtraced.
        /// </summary>
        public int Width { set; get; }
        /// <summary>
        /// The height of the image to be pathtraced.
        /// </summary>
        public int Height { set; get; }

        public int MaxDepth { set; get; }

        private readonly Shader shader;
        private readonly Model quad;
        private readonly int texture;

        public PathTracingRenderer( int width, int height )
        {
            MaxDepth = 3;

            shader = new Shader( );
            shader.AddShader( File.ReadAllText( "Shaders/texturedQuad.frag" ), ShaderType.FragmentShader );
            shader.AddShader( File.ReadAllText( "Shaders/texturedQuad.vert" ), ShaderType.VertexShader );
            shader.Link( );

            quad = new Model( );
            Mesh quadMesh = new Mesh( quad );
            quadMesh.SetTrianglesWithCollider( new[ ]
            {
                new Vertex { Position = new Vector3( -1, -1, 0 ), TexCoord = new Vector2( 0, 0 ) },
                new Vertex { Position = new Vector3( -1, 1, 0 ), TexCoord = new Vector2( 0, 1 ) },
                new Vertex { Position = new Vector3( 1, 1, 0 ), TexCoord = new Vector2( 1, 1 ) },
                new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ) }
            }, new[ ] { new Face { Vertices = new uint[ ] { 0, 1, 2, 2, 3, 0 } } } );
            quadMesh.Shader = shader;

            Width = width;
            Height = height;
            Util.CreateNullTexture( width, height, out texture );
        }

        private void SetColor( int x, int y, Color4 color )
        {
            int id = ( y * Width + x ) * 3;
            img[ id ] = ( byte ) ( Math.Min( 1f, color.R ) * 255f );
            img[ id + 1 ] = ( byte ) ( Math.Min( 1f, color.G ) * 255f );
            img[ id + 2 ] = ( byte ) ( Math.Min( 1f, color.B ) * 255f );
        }

        public override void Render( Camera cam, World world )
        {
            CheckBufferSize( Width, Height );

            List<CollisionObject> lights = new List<CollisionObject>( );
            List<CollisionObject> colliders = new List<CollisionObject>( );

            foreach( Model mdl in world.Models )
                foreach ( Mesh msh in mdl.Meshes )
                {
                    colliders.AddRange( msh.Colliders );
                    if ( msh.Material.Emission.ToArgb( ) > 0 )
                        lights.AddRange( msh.Colliders );
                }
            Parallel.For( 0, Width * Height, index =>
            //for( int index = 0; index < Width*Height; index++)
            {
                int x = index % Width;
                int y = index / Width;
                Ray ray = cam.GetRayFromPixel( x, y, Width, Height );

                SetColor( x, y, Trace( ray, colliders, lights ) );
            } );

            RenderBufferToTexture( );
        }

        private HitResult GetIntersection( Ray ray, List<CollisionObject> colliders )
        {
            HitResult closest = new HitResult( );
            foreach ( CollisionObject obj in colliders )
            {
                HitResult check = obj.Intersect( ray );
                if ( !closest.Hit && check.Hit || check.Hit && check.Distance < closest.Distance )
                    closest = check;
            }

            return closest;
        }

        private Color4 Radiance( HitResult result )
        {
            if ( !result.Hit )
                return Color4.Black;

            return result.Mesh.Material.Diffuse;
        }

        public Color4 Trace( Ray ray, List<CollisionObject> colliders, List<CollisionObject> lights )
        {
            HitResult closest = GetIntersection( ray, colliders );

            Color4 col = Color4.Black;
            for ( int x = 0; x < MaxDepth; x++ )
            {
                Color4 rad = Radiance( closest );
                col.R += rad.R;
                col.G += rad.G;
                col.B += rad.B;
                col.A += rad.A;

                if ( !closest.Hit )
                    break;

                ray = new Ray( closest.Position + closest.Normal * float.Epsilon,
                    PathTraceUtil.Reflect( ray.Direction, closest.Normal ) );
                closest = GetIntersection( ray, colliders );
            }

            return col;
        }

        private void RenderBufferToTexture( )
        {
            GL.BindTexture( TextureTarget.Texture2D, texture );
            GL.Clear( ClearBufferMask.ColorBufferBit );
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, img );

            shader.Use( );
            shader.SetTexture( "quadTexture", texture );
            quad.Render( );
        }

        private void CheckBufferSize( int width, int height )
        {
            int pixelCount = width * height;
            if ( img.Length != pixelCount * 3 )
                img = new byte[ pixelCount * 3 ];
        }
    }
}