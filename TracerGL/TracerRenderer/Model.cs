using System.Collections.Generic;
using TracerRenderer.Data;

namespace TracerRenderer
{
    /// <summary>
    /// A 3D model.
    /// </summary>
    public class Model
    {
        public List<Mesh> Meshes{ get; } = new List<Mesh>( );

        public Transform Transform { private set; get; }

        public Model( )
        {
            Transform = new Transform( );
        }

        public void AddMesh( Mesh m )
        {
            Meshes.Add( m );
        }

        public void Render( )
        {
            foreach ( Mesh mesh in Meshes )
                mesh.Render( );
        }
    }
}
