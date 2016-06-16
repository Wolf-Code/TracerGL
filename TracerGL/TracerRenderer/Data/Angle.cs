
using OpenTK;

namespace TracerRenderer.Data
{
    public class Angle
    {
        public float Pitch { set; get; }

        public float Yaw { set; get; }

        public float Roll { set; get; }

        public Angle( float pitch = 0, float yaw = 0, float roll = 0 )
        {
            this.Pitch = pitch;
            this.Yaw = yaw;
            this.Roll = roll;
        }

        public Quaternion Quaternion
        {
            get
            {
                Quaternion pitchRotation = Quaternion.FromAxisAngle( Vector3.UnitX, MathHelper.DegreesToRadians( Pitch ) );
                Quaternion yawRotation = Quaternion.FromAxisAngle( Vector3.UnitY, MathHelper.DegreesToRadians( Yaw ) );
                Quaternion rollRotation = Quaternion.FromAxisAngle( Vector3.UnitZ, MathHelper.DegreesToRadians( Roll ) );

                Quaternion rotation = pitchRotation * yawRotation * rollRotation;
                return rotation;
            }
        }

        public override string ToString( )
        {
            return $"(P: {Pitch}, Y: {Yaw}, R: {Roll})";
        }
    }
}
