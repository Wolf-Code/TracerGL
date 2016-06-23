using OpenTK;

namespace TracerRenderer
{
    class PathTraceUtil
    {
        public static Vector3 Reflect( Vector3 ray, Vector3 normal )
        {
            return ray - 2 * normal * Vector3.Dot( ray, normal );
        }

        public static Vector3 RandomDirection( )
        {
            return
                new Vector3( ThreadRandom.NextNegPosFloat( ), ThreadRandom.NextNegPosFloat( ),
                    ThreadRandom.NextNegPosFloat( ) ).Normalized( );
        }

        public static Vector3 RandomDirectionInSameHemisphere( Vector3 direction )
        {
            Vector3 rand = RandomDirection( );

            if ( Vector3.Dot( direction, rand ) < 0 )
                rand *= -1;

            return rand;
        }
    }
}
