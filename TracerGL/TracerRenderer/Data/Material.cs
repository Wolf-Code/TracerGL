using OpenTK.Graphics;

namespace TracerRenderer.Data
{
    /// <summary>
    /// Contains information about a material.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The material's diffuse colour.
        /// </summary>
        public Color4 Diffuse { set; get; } = Color4.White;
    }
}
