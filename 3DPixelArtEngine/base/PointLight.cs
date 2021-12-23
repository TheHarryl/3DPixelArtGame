using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vector3 = System.Numerics.Vector3;

namespace _3DPixelArtEngine
{
    public class PointLight
    {
        public bool Enabled;

        public Color Color;
        public float Intensity;
        public int LightQuantization;

        public float InnerRange;
        public float OuterRange;

        public Ray Orientation;
        public float LightSpread;

        public PointLight(Color color, float innerRange, float outerRange, float intensity = 1f, int lightQuantization = 3, float lightSpread = 180f)
        {
            Color = color;
            InnerRange = innerRange;
            OuterRange = outerRange;
            Intensity = intensity;
            LightQuantization = lightQuantization;
            LightSpread = lightSpread;

            Orientation = new Ray(Vector3.Zero, new Vector3(1f, 0f, 0f));
            Enabled = true;
        }

        public float GetIntensityAtDistance(float distance)
        {
            if (distance > OuterRange) return 0f;
            if (distance <= InnerRange) return Intensity;
            float layerLength = (OuterRange - InnerRange) / (LightQuantization - 1);
            int layer = (int)Math.Ceiling((distance - InnerRange) / layerLength);
            return (Intensity / LightQuantization) * layer;
        }
    }
}
