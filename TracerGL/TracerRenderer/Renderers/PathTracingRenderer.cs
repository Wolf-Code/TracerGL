using System;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.Data;

namespace TracerRenderer.Renderers
{
    public class PathTracingRenderer : Renderer
    {
        private byte[ ] img = new byte[ 0 ];

        public override void Render( Camera cam, World world )
        {
            Random r = new Random( );
            CheckBufferSize( cam );

            r.NextBytes( img );

            RenderBufferToTexture( cam.RenderTarget.Width, cam.RenderTarget.Height );
        }

        private void RenderBufferToTexture( int width, int height )
        {
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Bgr,
                PixelType.Byte, img );
        }

        private void CheckBufferSize( Camera cam )
        {
            int pixelCount = cam.RenderTarget.Width * cam.RenderTarget.Height;
            if ( img.Length != pixelCount * 3 )
                img = new byte[ pixelCount * 3 ];
        }
    }
}