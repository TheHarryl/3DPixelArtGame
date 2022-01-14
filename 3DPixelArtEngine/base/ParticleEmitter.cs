using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace _3DPixelArtEngine
{
    public class ParticleEmitter : ObjectProperty
    {
        public Ray Face;
        public Texture2D Texture;
        public ColorAnimation Color;
        public float Duration;
        public float EmissionRate;
        public float Spread;
        public bool LockedToObject;
        public NumberRange Velocity;
        public NumberRange Acceleration;

        public ParticleEmitter(Ray face, Texture2D particleTexture, ColorAnimation color, float duration, float emissionRate, float spreadInDegrees, bool lockedToObject, NumberRange velocity, NumberRange acceleration)
        {
            Face = face;
            Texture = particleTexture;
            Color = color;
            Duration = duration;
            EmissionRate = emissionRate;
            Spread = spreadInDegrees;
            LockedToObject = lockedToObject;
            Velocity = velocity;
            Acceleration = acceleration;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}
