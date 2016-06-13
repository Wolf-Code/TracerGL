using OpenTK;

namespace TracerRenderer.Data
{
    public class Transform
    {
        public Vector3 Position { set; get; }

        public Vector3 Forward => Vector3.Transform( Vector3.UnitZ, Rotation.Quaternion );
        public Vector3 Right => Vector3.Transform( -Vector3.UnitX, Rotation.Quaternion );
        public Vector3 Up => Vector3.Transform( Vector3.UnitY, Rotation.Quaternion );

        public Matrix4 GetMatrix( )
        {
            Vector3 axis;
            float ang;
            Rotation.Quaternion.ToAxisAngle( out axis, out ang );

            return Matrix4.CreateFromAxisAngle( axis, ang ) * Matrix4.CreateTranslation( Position ) * Matrix4.CreateScale( -1, 1, 1 );
        }

        public Angle Rotation { set; get; }

        public Transform( )
        {
            this.Rotation = new Angle( );
            this.Position = new Vector3( );
        }
    }
}
