using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Ray
    {
        public Vector3 Point;
        public Vector3 Direction;

        public Ray(Vector3 point, Vector3 direction)
        {
            Point = point;
            Direction = direction;
        }
    }
}
