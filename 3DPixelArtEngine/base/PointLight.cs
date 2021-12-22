using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class PointLight
    {
        public Color Color;
        public float Intensity;
        public int LightQuantization;

        public float InnerRange;
        public float OuterRange;

        public Ray Orientation;

        public PointLight()
        {

        }
    }
}
