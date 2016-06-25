using System;
using OpenTK;
using OpenTK.Graphics;

namespace TracerRenderer.Data
{
    /// <summary>
    /// Contains information about a material.
    /// </summary>
    public class Material
    {
        /// <summary>
        /// The material's diffuse colour.
        /// </summary>
        public Color Diffuse { set; get; } = new Color( 1f, 1f, 1f );

        /// <summary>
        /// The emission of the material.
        /// </summary>
        public Color Emission { set; get; } = new Color( );

        public enum MaterialType
        {
            Diffuse,
            Reflective
        }

        public MaterialType Type { get; set; } = MaterialType.Diffuse;

        public float CosTheta( Vector3 outDirection, Vector3 normal )
        {
            switch ( Type )
            {
                case MaterialType.Reflective:
                case MaterialType.Diffuse:
                    return Math.Abs( Vector3.Dot( outDirection, normal ) );
            }

            return 0f;
        }

        public float BRDF( Vector3 inDir, Vector3 outDir, Vector3 normal )
        {
            switch ( Type )
            {
                case MaterialType.Diffuse:
                    return ( float ) ( 1f / Math.PI );

                case MaterialType.Reflective:
                    if ( PathTraceUtil.Reflect( inDir, normal ) == outDir )
                        return 1.0f;

                    break;
            }

            return 0f;
        }

        public Vector3 GetNewRay( Vector3 normal, Vector3 inRay )
        {
            switch ( Type )
            {
                case MaterialType.Diffuse:
                    return PathTraceUtil.RandomDirectionInSameHemisphere( normal );

                case MaterialType.Reflective:
                    return PathTraceUtil.Reflect( inRay, normal );
            }

            return Vector3.Zero;
        }
    }
}
