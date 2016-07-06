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
        private float radius, radiusSquared;

        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public float Radius
        {
            set
            {
                radius = value;
                radiusSquared = radius * radius;
            }
            get { return radius; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="radius">The radius of the sphere.</param>
        public Sphere( float radius )
        {
            Radius = radius;
        }

        /// <summary>
        /// Checks for intersection between this <see cref="CollisionObject"/> and a <see cref="Ray"/>.
        /// </summary>
        /// <param name="ray">The <see cref="Ray"/> to check with for intersection.</param>
        /// <returns>A <see cref="HitResult"/> containing all information about the test.</returns>
        public override HitResult Intersect( Ray ray )
        {
            HitResult res = new HitResult( );

            float A = Vector3.Dot( ray.Direction, ray.Direction );
            float B = 2 * Vector3.Dot( ray.Direction, ray.Start - this.Transform.WorldPosition );
            float C = ( ray.Start - this.Transform.WorldPosition ).LengthSquared - radiusSquared;

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
            res.Normal = ( res.Position - this.Transform.WorldPosition ).Normalized( );
            res.Mesh = this.Mesh;

            return res;
        }
    }
}