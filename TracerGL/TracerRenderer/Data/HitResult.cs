﻿using OpenTK;

namespace TracerRenderer.Data
{
    public class HitResult
    {
        public bool Hit { set; get; }
        public float Distance { set; get; }
        public Mesh Mesh { set; get; }
        public Vector3 Position { set; get; }
        public Vector3 Normal { set; get; }
    }
}
