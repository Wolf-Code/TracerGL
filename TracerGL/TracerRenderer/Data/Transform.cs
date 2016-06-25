using System;
using OpenTK;

namespace TracerRenderer.Data
{
    public class Transform
    {
        private Vector3 position;
        public Transform Parent { set; get; }

        public Vector3 Position
        {
            set
            {
                if ( position == value ) return;

                position = value;
                OnPositionChanged?.Invoke( this, position );
            }
            get { return position; }
        }

        public Vector3 WorldPosition => Vector3.Transform( this.position, this.GetMatrix( ) );

        public EventHandler<Vector3> OnPositionChanged;
        public EventHandler<Angle> OnRotationChanged;  

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
            this.Rotation.OnChange += ( sender, args ) => this.OnRotationChanged?.Invoke( this, ( Angle ) sender );
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
