using OpenTK;

namespace TracerRenderer
{
    class PathTraceUtil
    {
        public static Vector3 Reflect( Vector3 ray, Vector3 normal )
        {
            return ray - 2 * normal * Vector3.Dot( ray, normal );
        }
    }
}
