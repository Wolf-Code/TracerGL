using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using TracerRenderer.CollisionObjects;
using TracerRenderer.Data;

namespace TracerRenderer
{
    /// <summary>
    /// Contains utility methods.
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Creates an empty texture.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="texture">The texture ID.</param>
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

        public static Model CreateSphere( float radius, byte segments, byte rings )
        {
            Vertex[ ] data = new Vertex[ segments * rings ];

            int i = 0;

            for ( double y = 0; y < rings; y++ )
            {
                double phi = ( y / ( rings - 1 ) ) * Math.PI; //was /2 
                for ( double x = 0; x < segments; x++ )
                {
                    double theta = ( x / ( segments - 1 ) ) * 2 * Math.PI;

                    Vector3 v = new Vector3(
                        ( float ) ( radius * Math.Sin( phi ) * Math.Cos( theta ) ),
                        ( float ) ( radius * Math.Cos( phi ) ),
                        ( float ) ( radius * Math.Sin( phi ) * Math.Sin( theta ) )
                        );
                    Vector3 n = Vector3.Normalize( v );
                    Vector2 uv = new Vector2(
                        ( float ) ( x / ( segments - 1 ) ),
                        ( float ) ( y / ( rings - 1 ) )
                        );

                    data[ i++ ] = new Vertex { Position = v, Normal = n, TexCoord = uv };
                }
            }

            uint[ ] elements = CalculateElements( segments, rings );

            Model m = new Model( data, elements );
            Sphere sph = new Sphere( radius );
            m.AddCollisionObject( sph );

            return m;
        }

        private static uint[ ] CalculateElements( byte segments, byte rings )
        {
            int num_vertices = segments * rings;
            uint[ ] data = new uint[ num_vertices * 6 ];

            ushort i = 0;

            for ( byte y = 0; y < rings - 1; y++ )
            {
                for ( byte x = 0; x < segments - 1; x++ )
                {
                    data[ i++ ] = ( uint ) ( ( y + 0 ) * segments + x );
                    data[ i++ ] = ( uint ) ( ( y + 1 ) * segments + x );
                    data[ i++ ] = ( uint ) ( ( y + 1 ) * segments + x + 1 );

                    data[ i++ ] = ( uint ) ( ( y + 1 ) * segments + x + 1 );
                    data[ i++ ] = ( uint ) ( ( y + 0 ) * segments + x + 1 );
                    data[ i++ ] = ( uint ) ( ( y + 0 ) * segments + x );
                }
            }

            // Verify that we don't access any vertices out of bounds:
            foreach ( int index in data )
                if ( index >= segments * rings )
                    throw new IndexOutOfRangeException( );

            return data;
        }
    }
}
