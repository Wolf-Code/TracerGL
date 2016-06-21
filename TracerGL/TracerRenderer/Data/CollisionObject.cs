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
        /// The parent model.
        /// </summary>
        public Model Model { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected CollisionObject()
        {
            this.Transform = new Transform();
        }

        /// <summary>
        /// Sets the parent of the <see cref="CollisionObject"/>.
        /// </summary>
        /// <param name="mdl">The <see cref="Model"/> to use as parent.</param>
        public void SetParent( Model mdl )
        {
            this.Model = mdl;
            this.Transform.Parent = mdl.Transform;
        }

        /// <summary>
        /// Checks for intersection between this <see cref="CollisionObject"/> and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The <see cref="Ray"/> to check with for intersection.</param>
        /// <returns>A <see cref="HitResult"/> containing all information about the test.</returns>
        public abstract HitResult Intersect( Ray ray );
    }
}