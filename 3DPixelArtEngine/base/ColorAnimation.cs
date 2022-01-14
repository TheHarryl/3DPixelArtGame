using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace _3DPixelArtEngine
{
    public struct ColorAnimation
    {
        public List<ColorAnimationStep> AnimationSteps;

        public ColorAnimation(List<ColorAnimationStep> animationSteps)
        {
            AnimationSteps = animationSteps;
        }

        public Color GetValueAtTime(float interpolant)
        {
            if (AnimationSteps.Count == 0) return Color.White;
            for (int i = 0; i < AnimationSteps.Count - 1; i++)
            {
                if (AnimationSteps[i].Time <= interpolant) continue;
                return Color.Lerp(AnimationSteps[i].Color, AnimationSteps[i + 1].Color, (interpolant - AnimationSteps[i].Time) / (AnimationSteps[i + 1].Time - AnimationSteps[i].Time));
            }
            return AnimationSteps[AnimationSteps.Count - 1].Color;
        }
    }
}
