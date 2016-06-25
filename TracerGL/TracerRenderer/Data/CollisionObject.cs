using OpenTK;

namespace TracerRenderer.Data
{
    /// <summary>
    /// A collision object which contains a method to check for intersection with a ray.
    /// </summary>
    public abstract class CollisionObject
    {
        /// <summary>
        /// The transform of this <see cref="CollisionObject"/>.
        /// </summary>
        public Transform Transform { private set; get; }
        
        /// <summary>
        /// The world position of the collision object.
        /// </summary>
        protected Vector3 WorldPosition => Vector3.Transform( this.Transform.Position, this.Mesh.Transform.GetMatrix( ) );

        /// <summary>
        /// The parent model.
        /// </summary>
        public Mesh Mesh{ private set; get; }

        protected CollisionObject( )
        {
            this.Transform = new Transform( );
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CollisionObject( Mesh mesh ) : this( )
        {
            this.SetParent( mesh );
        }


        /// <summary>
        /// Sets the parent of the <see cref="CollisionObject"/>.
        /// </summary>
        /// <param name="mesh">The <see cref="Mesh"/> to use as parent.</param>
        public void SetParent( Mesh mesh )
        {
            mesh.AddCollider( this );
            this.Mesh = mesh;
            this.Transform.Parent = mesh.Transform;
        }

        /// <summary>
        /// Checks for intersection between this <see cref="CollisionObject"/> and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The <see cref="Ray"/> to check with for intersection.</param>
        /// <returns>A <see cref="HitResult"/> containing all information about the test.</returns>
        public abstract HitResult Intersect( Ray ray );
    }
}