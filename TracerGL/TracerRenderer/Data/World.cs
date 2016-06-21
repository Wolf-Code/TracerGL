using System.Collections.Generic;

namespace TracerRenderer.Data
{
    /// <summary>
    /// Contains information about the world.
    /// </summary>
    public class World
    {
        /// <summary>
        /// The models inside the world.
        /// </summary>
        public List<Model> Models { get; } = new List<Model>( );

        /// <summary>
        /// Adds a model to the world.
        /// </summary>
        /// <param name="mdl">The <see cref="Model"/> to add to the world.</param>
        public void AddModel( Model mdl )
        {
            this.Models.Add( mdl );
        }
    }
}
