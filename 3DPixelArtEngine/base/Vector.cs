using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Vector : Ray
    {
        public float Length;

        public Vector(Vector3 point, Vector2 rotation, float length) : base(point, rotation)
        {
            Length = length;
        }

        public Vector(Vector3 start, Vector3 end) : base(start, new Vector2((float)Math.Atan((start.Y - end.Y) / (start.X - end.X)), (float)Math.Atan((start.Y - end.Y) / (start.Z - end.Z))))
        {
            Length = Vector3.Distance(start, end);
        }
    }
}
