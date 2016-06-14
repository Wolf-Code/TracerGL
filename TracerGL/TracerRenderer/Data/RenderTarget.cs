using System;
using OpenTK.Graphics.OpenGL;

namespace TracerRenderer.Data
{
    public class RenderTarget
    {
        private readonly int frameBuffer;

        public int ColorTexture { private set; get; }

        public int DepthTexture { private set; get; }

        private readonly int width;
        private readonly int height;

        public RenderTarget( int width, int height )
        {
            this.width = width;
            this.height = height;

            int colorTexture, depthBuffer;
            CreateNullTexture( out colorTexture );
            CreateDepthTexture( out depthBuffer );
            // create and bind an FBO
            GL.Ext.GenFramebuffers( 1, out frameBuffer );
            GL.Ext.BindFramebuffer( FramebufferTarget.DrawFramebuffer, frameBuffer );

            // assign texture to FBO
            GL.Ext.FramebufferTexture2D( FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, colorTexture, 0 );
            GL.Ext.FramebufferRenderbuffer( FramebufferTarget.DrawFramebuffer, FramebufferAttachment.DepthAttachmentExt, RenderbufferTarget.RenderbufferExt, depthBuffer );

            #region Test for Error

            switch ( GL.Ext.CheckFramebufferStatus( FramebufferTarget.FramebufferExt ) )
            {
                case FramebufferErrorCode.FramebufferCompleteExt:
                    {
                        Console.WriteLine( "FBO: The framebuffer " + frameBuffer + " is complete and valid for rendering." );
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteAttachmentExt:
                    {
                        Console.WriteLine( "FBO: One or more attachment points are not framebuffer attachment complete. This could mean there’s no texture attached or the format isn’t renderable. For color textures this means the base format must be RGB or RGBA and for depth textures it must be a DEPTH_COMPONENT format. Other causes of this error are that the width or height is zero or the z-offset is out of range in case of render to volume." );
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteMissingAttachmentExt:
                    {
                        Console.WriteLine( "FBO: There are no attachments." );
                        break;
                    }
                /* case  FramebufferErrorCode.GL_FRAMEBUFFER_INCOMPLETE_DUPLICATE_ATTACHMENT_EXT: 
                     {
                         Console.WriteLine("FBO: An object has been attached to more than one attachment point.");
                         break;
                     }*/
                case FramebufferErrorCode.FramebufferIncompleteDimensionsExt:
                    {
                        Console.WriteLine( "FBO: Attachments are of different size. All attachments must have the same width and height." );
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteFormatsExt:
                    {
                        Console.WriteLine( "FBO: The color attachments have different format. All color attachments must have the same format." );
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteDrawBufferExt:
                    {
                        Console.WriteLine( "FBO: An attachment point referenced by GL.DrawBuffers() doesn’t have an attachment." );
                        break;
                    }
                case FramebufferErrorCode.FramebufferIncompleteReadBufferExt:
                    {
                        Console.WriteLine( "FBO: The attachment point referenced by GL.ReadBuffers() doesn’t have an attachment." );
                        break;
                    }
                case FramebufferErrorCode.FramebufferUnsupportedExt:
                    {
                        Console.WriteLine( "FBO: This particular FBO configuration is not supported by the implementation." );
                        break;
                    }
                default:
                    {
                        Console.WriteLine( "FBO: Status unknown. (yes, this is really bad.)" );
                        break;
                    }
            }

            #endregion Test for Error

            GL.Ext.BindFramebuffer( FramebufferTarget.DrawFramebuffer, 0 ); // disable rendering into the FBO

            ColorTexture = colorTexture;
            DepthTexture = DepthTexture;
        }

        private void CreateNullTexture( out int texture )
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

        private void CreateDepthTexture( out int buffer )
        {
            GL.Ext.GenRenderbuffers( 1, out buffer );
            GL.Ext.BindRenderbuffer( RenderbufferTarget.RenderbufferExt, buffer );
            GL.Ext.RenderbufferStorage( RenderbufferTarget.RenderbufferExt, ( RenderbufferStorage )All.DepthComponent32, width, height );
        }

        public void Bind( )
        {
            GL.Ext.BindFramebuffer( FramebufferTarget.DrawFramebuffer, frameBuffer );
        }

        public void Unbind( )
        {
            GL.Ext.BindFramebuffer( FramebufferTarget.DrawFramebuffer, 0 );
        }
    }
}
