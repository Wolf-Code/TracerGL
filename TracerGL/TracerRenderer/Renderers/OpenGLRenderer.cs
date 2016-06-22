using OpenTK;
using TracerRenderer.Data;

namespace TracerRenderer.Renderers
{
    public class OpenGLRenderer : Renderer
    {
        public override void Render( Camera cam, World world )
        {
            Matrix4 VP = cam.GetMatrix( ) * cam.Projection;

            foreach ( Model mdl in world.Models )
            {
                foreach ( Mesh mesh in mdl.Meshes )
                {
                    mesh.Shader.Use( );
                    mesh.Shader.SetMatrix( "MVP", mdl.Transform.GetMatrix( ) * VP );
                    mdl.Render( );
                }
            }
        }
    }
}
