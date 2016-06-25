using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace TracerRenderer.Data
{
    public class Color
    {
        public float R { set; get; }

        public float G { set; get; }

        public float B { set; get; }

        public bool HasValue => R > 0 || G > 0 || B > 0;

        public Color( float r = 0, float g = 0, float b = 0 )
        {
            R = r;
            G = g;
            B = b;
        }

        public Color( Vector3 vec ) : this( vec.X, vec.Y, vec.Z )
        {
            
        }

        public static Color operator +( Color col1, Color col2 )
        {
            return new Color( col1.R + col2.R, col1.G + col2.G, col1.B + col2.B );
        }

        public static Color operator *( Color col1, Color col2 )
        {
            return new Color( col1.R * col2.R, col1.G * col2.G, col1.B * col2.B );
        }

        public static Color operator *( Color col, float mul )
        {
            return new Color( col.R * mul, col.G * mul, col.B * mul );
        }

        public Color4 ToColor4( )
        {
            return new Color4( R, G, B, 1f );
        }
    }
}
