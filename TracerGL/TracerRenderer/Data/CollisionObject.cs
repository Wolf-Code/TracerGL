namespace TracerRenderer.Data
{
    public abstract class CollisionObject
    {
        public Transform Transform { private set; get; }

        protected CollisionObject()
        {
            this.Transform = new Transform();
        }

        public abstract HitResult Intersect( Ray ray );
    }
}