using System;
using OpenTK;
using TracerRenderer.Data;

namespace TracerRenderer.CollisionObjects
{
    public class Triangle : CollisionObject
    {
        const float epsilon = 0.0001f;
        public Model Model { set; get; }
        public Vertex V1 { set; get; }
        public Vertex V2 { set; get; }
        public Vertex V3 { set; get; }

        public override HitResult Intersect( Ray ray )
        {
            HitResult Res = new HitResult { Hit = false };
            Matrix4 mtx = Transform.GetMatrix( );

            Vector3 v1 = Vector3.Transform( V1.Position, mtx );
            Vector3 v2 = Vector3.Transform( V2.Position, mtx );
            Vector3 v3 = Vector3.Transform( V3.Position, mtx );

            Vector3 e1 = v2 - v1;
            Vector3 e2 = v3 - v1;
            Vector3 q = Vector3.Cross( ray.Direction, e2 );
            float a = Vector3.Dot( e1, q );
            //if(a < 0) return false; // Backface cull
            if ( a > -epsilon && a < epsilon ) return Res;

            float f = 1.0f / a;
            Vector3 s = ray.Start - V1.Position;
            float u = f * Vector3.Dot( s, q );
            if ( u < 0.0 || u > 1.0 ) return Res;
            
            Vector3 _R = Vector3.Cross( s, e1 );
            float v = f * Vector3.Dot( ray.Direction, _R );
            if ( v < 0.0 || u + v > 1.0 ) return Res;

            float t = f * Vector3.Dot( e2, _R );

            if ( t < epsilon ) return Res;

            Res.Distance = t;
            Res.Position = ray.Start + ray.Direction * t;
            float w = 1.0f - ( u + v );
            Res.Normal = Vector3.Normalize( w * V1.Normal + u * V2.Normal + v * V3.Normal );

            if ( Vector3.Dot( ray.Direction, Res.Normal ) > 0 )
                Res.Normal = Res.Normal * -1;

            Res.Hit = true;
            Res.Model = Model;

            return Res;
        }
    }
}
