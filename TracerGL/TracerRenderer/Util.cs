using System;
using OpenTK.Graphics.OpenGL;

namespace TracerRenderer
{
    public static class Util
    {
        public static void CreateNullTexture( int width, int height, out int texture )
        {
            // load texture 
            GL.GenTextures( 1, out texture );

            // Still required else TexImage2D will be applyed on the last bound texture
            GL.BindTexture( TextureTarget.Texture2D, texture );

            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int )TextureMinFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int )TextureMagFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int )TextureWrapMode.Clamp );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int )TextureWrapMode.Clamp );

            // generate null texture
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero );

            GL.BindTexture( TextureTarget.Texture2D, 0 );
        }
    }
}
