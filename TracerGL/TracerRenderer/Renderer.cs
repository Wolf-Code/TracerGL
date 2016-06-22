using TracerRenderer.Data;

namespace TracerRenderer
{
    /// <summary>
    /// Responsible for rendering a scene.
    /// </summary>
    public abstract class Renderer
    {
        /// <summary>
        /// Renders the scene given a <see cref="Camera"/> and a <see cref="World"/>.
        /// </summary>
        /// <param name="cam">The camera to render from.</param>
        /// <param name="world">The world.</param>
        public abstract void Render( Camera cam, World world );
    }
}
