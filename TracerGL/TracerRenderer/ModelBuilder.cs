using System.Collections.Generic;
using TracerRenderer.Data;

namespace TracerRenderer
{
    public class ModelBuilder
    {
        public List<Vertex> Vertices { private set; get; } = new List<Vertex>( );
        public List<Face> Faces { private set; get; } = new List<Face>( );

        public void AddVertex( Vertex v )
        {
            this.Vertices.Add( v );
        }

        public void AddFace( Face f )
        {
            this.Faces.Add( f );
        }

        public Model GetModel( )
        {
            return new Model( Vertices.ToArray( ), Faces.ToArray( ) );
        }
    }
}
