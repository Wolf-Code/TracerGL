using OpenTK;
using TracerRenderer.Data;

namespace TracerRenderer
{
    public class Camera
    {
        public Transform Transform { set; get; }

        public Matrix4 Projection { private set; get; }

        private float m_aspect, m_fov, m_near, m_far;

        public Matrix4 GetMatrix( ) => Matrix4.LookAt( Transform.Position, Transform.Position + Transform.Forward, Transform.Up );

        public RenderTarget RenderTarget { private set; get; }

        public Camera( int width, int height )
        {
            this.m_aspect = 1;
            this.m_fov = 70;
            this.m_near = 1;
            this.m_far = 1000;
            this.Update( );

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
            this.Update( );
        }

        public void SetNearFar( float near, float far )
        {
            this.m_near = near;
            this.m_far = far;
        }

        private void Update( )
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView( MathHelper.DegreesToRadians( m_fov ), m_aspect, m_near, m_far );
        }

        public void BindForRendering( )
        {
            RenderTarget.Bind( );
        }
    }
}
