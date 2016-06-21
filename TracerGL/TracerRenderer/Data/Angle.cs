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

        public Quaternion Rotation { private set; get; }

        public Angle( float pitch = 0, float yaw = 0, float roll = 0 )
        {
            this.Pitch = pitch;
            this.Yaw = yaw;
            this.Roll = roll;

            this.UpdateRotation( );
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

            this.UpdateRotation( );
        }

        private void UpdateRotation( )
        {
            Quaternion qP = Quaternion.FromAxisAngle( Vector3.UnitX, MathHelper.DegreesToRadians( this.Pitch ) );
            Quaternion qY = Quaternion.FromAxisAngle( Vector3.UnitY, MathHelper.DegreesToRadians( this.Yaw ) );
            Quaternion qR = Quaternion.FromAxisAngle( Vector3.UnitZ, MathHelper.DegreesToRadians( this.Roll ) );
            this.Rotation = qR * qY * qP;

            Right = Vector3.Transform( Vector3.UnitX, this.Rotation );
            Up = Vector3.Transform( Vector3.UnitY, this.Rotation );
            Forward = Vector3.Transform( -Vector3.UnitZ, this.Rotation );
        }

        public void GetRotation( out Vector3 axis, out float angle )
        {
            this.Rotation.ToAxisAngle( out axis, out angle );
        }

        public override string ToString( )
        {
            return $"(P: {Pitch}, Y: {Yaw}, R: {Roll})";
        }
    }
}
