using System;
using OpenTK;

namespace TracerRenderer.Data
{
    /// <summary>
    /// Keeps track of a position and rotation.
    /// </summary>
    public class Transform
    {
        private Vector3 position;

        /// <summary>
        /// The transform's parent.
        /// </summary>
        public Transform Parent { set; get; }

        /// <summary>
        /// The local position of the transform.
        /// </summary>
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

        /// <summary>
        /// The position of the <see cref="Transform"/> in world coordinates.
        /// </summary>
        public Vector3 WorldPosition => Vector3.Transform( this.position, this.GetMatrix( ) );

        /// <summary>
        /// Gets called whenever the position is changed.
        /// </summary>
        public EventHandler<Vector3> OnPositionChanged;

        /// <summary>
        /// Gets called whenever the rotation is changed.
        /// </summary>
        public EventHandler<Angle> OnRotationChanged;  

        /// <summary>
        /// The forward vector.
        /// </summary>
        public Vector3 Forward => Rotation.Forward;

        /// <summary>
        /// The right vector.
        /// </summary>
        public Vector3 Right => Rotation.Right;

        /// <summary>
        /// The up vector.
        /// </summary>
        public Vector3 Up => Rotation.Up;

        /// <summary>
        /// The matrix of the transform, containing its rotation and translation.
        /// </summary>
        /// <returns>The <see cref="Matrix4"/>.</returns>
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

        /// <summary>
        /// The rotation of the transform.
        /// </summary>
        public Angle Rotation { set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Transform( )
        {
            this.Rotation = new Angle( );
            this.Rotation.OnChange += ( sender, args ) => this.OnRotationChanged?.Invoke( this, ( Angle ) sender );
            this.Position = new Vector3( );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">The transform's position.</param>
        public Transform( Vector3 position ) : this( )
        {
            this.Position = position;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="angle">The transform's angle.</param>
        public Transform( Angle angle ) : this( )
        {
            this.Rotation = angle;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="position">The transform's position.</param>
        /// <param name="angle">The transform's angle.</param>
        public Transform( Vector3 position, Angle angle ) : this( )
        {
            this.Position = position;
            this.Rotation = angle;
        }
    }
}
