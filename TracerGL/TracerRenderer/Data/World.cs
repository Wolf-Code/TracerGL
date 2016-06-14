using System.Collections.Generic;

namespace TracerRenderer.Data
{
    public class World
    {
        public List<Model> Models { get; } = new List<Model>( );

        public void AddModel( Model mdl )
        {
            this.Models.Add( mdl );
        }
    }
}
