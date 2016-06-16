using System;
using OpenTK;
using TracerRenderer.Data;

namespace TracerRenderer
{
    public class Camera
    {
        public Transform Transform { set; get; }

        public Matrix4 Projection { private set; get; }

        private float m_aspect, m_fov, m_near, m_far;
        private float fovDivided;

        public Matrix4 GetMatrix( )
            => Matrix4.LookAt( Transform.Position, Transform.Position + Transform.Forward, Transform.Up );

        public RenderTarget RenderTarget { private set; get; }

        public Camera( int width, int height )
        {
            this.m_aspect = 1.0f;
            this.m_near = 1;
            this.m_far = 1000;
            this.SetFOV( 70 );

            RenderTarget = new RenderTarget( width, height );
        }

        public void SetAspect( float aspect )
        {
            this.m_aspect = aspect;
            this.Update( );
        }

        public void SetFOV( float fov )
        {
            this.m_fov = fov;
            this.fovDivided = -0.5f / ( float ) Math.Tan( MathHelper.DegreesToRadians( fov / 2 ) );
            this.Update( );
        }

        public void SetNearFar( float near, float far )
        {
            this.m_near = near;
            this.m_far = far;
            this.Update( );
        }

        private void Update( )
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView( MathHelper.DegreesToRadians( m_fov ), m_aspect, m_near,
                m_far );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The X-coordinate on the screen.</param>
        /// <param name="y">The Y-coordinate on the screen.</param>
        /// <param name="width">The width of the screen.</param>
        /// <param name="height">The height of the screen.</param>
        /// <returns></returns>
        public Ray GetRayFromPixel( float x, float y, int width, int height )
        {
            Vector3 dir = -this.Transform.Forward * fovDivided +
                          -this.Transform.Right * ( x / width - 0.5f ) *
                          this.m_aspect -
                          -this.Transform.Up * ( y / height - 0.5f );
            dir = dir.Normalized( );

            return new Ray( this.Transform.Position, dir );
        }

        public void BindForRendering( )
        {
            RenderTarget.Bind( );
        }
    }
}