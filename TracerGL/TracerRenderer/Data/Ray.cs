using OpenTK;

namespace TracerRenderer.Data
{
    public class Ray
    {
        public Vector3 Start { set; get; }
        public Vector3 Direction { set; get; }

        public Ray(Vector3 start, Vector3 direction)

        {
            this.Start = start;
            this.Direction = direction;
        }
    }
}
