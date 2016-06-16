using System;
using System.IO;
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
            for ( int x = 0; x < img.Length; x++ )
                img[ x ] = 127;

            for( int x = 0; x < Width; x++ )
                for ( int y = 0; y < Height; y++ )
                {
                    Ray ray = cam.GetRayFromPixel( x, y, Width, Height );
                    
                    SetColor( x, y, new Color4( Math.Abs( ray.Direction.X ), Math.Abs( ray.Direction.Y ), Math.Abs( ray.Direction.Z ), 1 ) );
                }

            RenderBufferToTexture( );
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