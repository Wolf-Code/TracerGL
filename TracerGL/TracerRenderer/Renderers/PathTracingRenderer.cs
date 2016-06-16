using System;
using System.IO;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.Data;

namespace TracerRenderer.Renderers
{
    public class PathTracingRenderer : Renderer
    {
        private byte[ ] img = new byte[ 0 ];

        public int Width { set; get; }
        public int Height { set; get; }

        private Shader shader;
        private Model quad;
        private int texture;

        public PathTracingRenderer( int width, int height )
        {
            shader = new Shader( );
            shader.AddShader( File.ReadAllText( "Shaders/texturedQuad.frag" ), ShaderType.FragmentShader );
            shader.AddShader( File.ReadAllText( "Shaders/texturedQuad.vert" ), ShaderType.VertexShader );
            shader.Link( );

            quad = new Model( new[ ]
            {
                new Vertex { Position = new Vector3( -1, -1, 0 ), TexCoord = new Vector2( 0, 0 ) },
                new Vertex { Position = new Vector3( -1, 1, 0 ), TexCoord = new Vector2( 0, 1 ) },
                new Vertex { Position = new Vector3( 1, 1, 0 ), TexCoord = new Vector2( 1, 1 ) },
                new Vertex { Position = new Vector3( 1, -1, 0 ), TexCoord = new Vector2( 1, 0 ) }
            }, new[ ] { new Face { Vertices = new uint[ ] { 0, 1, 2, 2, 3, 0 } } } )
            { Shader = shader };
            
            Width = width;
            Height = height;
            Util.CreateNullTexture( width, height, out texture );
        }

        private void SetColor( int x, int y, Color4 color )
        {
            int id = ( y * Width + x ) * 3;
            img[ id ] = ( byte )( color.R * 255f );
            img[ id + 1 ] = ( byte )( color.G * 255f );
            img[ id + 2 ] = ( byte )( color.B * 255f );
        }

        public override void Render( Camera cam, World world )
        {
            CheckBufferSize( Width, Height );

            Parallel.For( 0, Width * Height, index =>
            {
                int x = index % Width;
                int y = index / Width;
                Ray ray = cam.GetRayFromPixel( x, y, Width, Height );

                SetColor( x, y, Trace( ray, world ) );
            } );

            RenderBufferToTexture( );
        }

        public Color4 Trace( Ray ray, World world )
        {
            HitResult closest = new HitResult( );
            foreach ( Model mdl in world.Models )
            {
                foreach ( CollisionObject obj in mdl.Triangles )
                {
                    HitResult check = obj.Intersect( ray );
                    if ( !closest.Hit || check.Hit && check.Distance < closest.Distance )
                        closest = check;
                }
            }

            return new Color4( closest.Position.X, closest.Position.Y, closest.Position.Z, 1 );
        }

        private void RenderBufferToTexture( )
        {
            GL.BindTexture( TextureTarget.Texture2D, texture );
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Bgr,
    PixelType.Byte, img );

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