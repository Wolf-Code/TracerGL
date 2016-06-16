namespace TracerRenderer.Data
{
    public abstract class CollisionObject
    {
        public Transform Transform { private set; get; }

        protected CollisionObject()
        {
            this.Transform = new Transform();
        }
    }
}