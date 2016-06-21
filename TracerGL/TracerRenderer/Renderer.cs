using TracerRenderer.Data;

namespace TracerRenderer
{
    public abstract class Renderer
    {
        public abstract void Render( Camera cam, World world );
    }
}
