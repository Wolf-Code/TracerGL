﻿using System;
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

        public float CosTheta( Vector3 outDirection, Vector3 normal )
        {
            return Math.Abs( Vector3.Dot( outDirection, normal ) );
        }
    }
}
