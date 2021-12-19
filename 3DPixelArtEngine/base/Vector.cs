using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Vector : Ray
    {
        public float Length;

        public Vector(Vector3 point, Vector3 rotation, float length) : base(point, rotation)
        {
            Length = length;
        }
    }
}
