using System;
using OpenTK;
using TracerRenderer.Data;

namespace TracerRenderer.CollisionObjects
{
    /// <summary>
    /// A sphere collider.
    /// </summary>
    public class Sphere : CollisionObject
    {
        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public float Radius { set; get; }

        public Sphere( float radius )
        {
            Radius = radius;
        }

        public override HitResult Intersect( Ray ray )
        {
            HitResult res = new HitResult( );

            float A = Vector3.Dot( ray.Direction, ray.Direction );
            float B = 2 * Vector3.Dot( ray.Direction, ray.Start - this.Transform.Position );
            float C = ( ray.Start - this.Transform.Position ).LengthSquared - ( this.Radius * this.Radius );

            float Discriminant = B * B - 4 * A * C;
            if ( Discriminant < 0 )
                return res;

            float DiscriminantSqrt = ( float )Math.Sqrt( Discriminant );
            float Q;
            if ( B < 0 )
                Q = ( -B - DiscriminantSqrt ) / 2f;
            else
                Q = ( -B + DiscriminantSqrt ) / 2f;

            float T0 = Q / A;
            float T1 = C / Q;

            if ( T0 > T1 )
            {
                float TempT0 = T0;
                T0 = T1;
                T1 = TempT0;
            }

            // Sphere is behind the ray's start position.
            if ( T1 < 0 )
                return res;

            res.Distance = T0 < 0 ? T1 : T0;
            res.Hit = true;
            res.Position = ray.Start + ray.Direction * res.Distance;
            res.Normal = ( res.Position - this.Transform.Position ).Normalized( );
            res.Mesh = this.Mesh;

            return res;
        }
    }
}