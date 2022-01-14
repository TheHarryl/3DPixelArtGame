using Microsoft.Xna.Framework;

namespace _3DPixelArtEngine
{
    public struct ColorAnimationStep
    {
        public Color Color;
        public float Time;

        public ColorAnimationStep(Color color, float time)
        {
            Color = color;
            Time = time;
        }
    }
}
