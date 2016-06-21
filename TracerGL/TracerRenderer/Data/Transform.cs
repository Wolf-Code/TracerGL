using OpenTK;

namespace TracerRenderer.Data
{
    public class Transform
    {
        public Transform Parent { set; get; }
        public Vector3 Position { set; get; }

        public Vector3 Forward => Rotation.Forward;
        public Vector3 Right => Rotation.Right;
        public Vector3 Up => Rotation.Up;

        public Matrix4 GetMatrix( )
        {
            Matrix4 pMatrix = Matrix4.Identity;

            if ( Parent != null )
                pMatrix = Parent.GetMatrix( );

            Vector3 axis;
            float ang;
            
            this.Rotation.GetRotation( out axis, out ang );
            return Matrix4.CreateFromAxisAngle( axis, ang ) * Matrix4.CreateTranslation( Position ) * pMatrix;
        }

        public Angle Rotation { set; get; }

        public Transform( )
        {
            this.Rotation = new Angle( );
            this.Position = new Vector3( );
        }

        public Transform( Vector3 position ) : this( )
        {
            this.Position = position;
        }

        public Transform( Angle angle ) : this( )
        {
            this.Rotation = angle;
        }

        public Transform( Vector3 position, Angle angle ) : this( )
        {
            this.Position = position;
            this.Rotation = angle;
        }
    }
}
