using OpenTK;

namespace TracerRenderer.Data
{
    public class Angle
    {
        public float Pitch { private set; get; }

        public float Yaw { private set; get; }

        public float Roll { private set; get; }

        public Vector3 Forward { private set; get; }

        public Vector3 Right { private set; get; }

        public Vector3 Up { private set; get; }

        public Matrix4 Matrix { private set; get; }

        public Angle( float pitch = 0, float yaw = 0, float roll = 0 )
        {
            this.Pitch = pitch;
            this.Yaw = yaw;
            this.Roll = roll;

            this.UpdateMatrix( );
        }

        public void AddRotation( float pitch = 0, float yaw = 0, float roll = 0 )
        {
            this.SetRotation( this.Pitch + pitch, this.Yaw + yaw, this.Roll + roll );
        }

        public void SetRotation( float pitch = 0, float yaw = 0, float roll = 0 )
        {
            this.Pitch = pitch;
            this.Yaw = yaw;
            this.Roll = roll;

            this.UpdateMatrix( );
        }

        private void UpdateMatrix( )
        {
            this.Matrix = Matrix4.CreateRotationX( MathHelper.DegreesToRadians( Pitch ) ) *
                          Matrix4.CreateRotationY( MathHelper.DegreesToRadians( Yaw ) ) *
                          Matrix4.CreateRotationZ( MathHelper.DegreesToRadians( Roll ) );

            Forward = this.Matrix.Column2.Xyz.Normalized( );
            Right = this.Matrix.Column0.Xyz.Normalized( );
            Up = this.Matrix.Column1.Xyz.Normalized( );
        }

        public override string ToString( )
        {
            return $"(P: {Pitch}, Y: {Yaw}, R: {Roll})";
        }
    }
}
