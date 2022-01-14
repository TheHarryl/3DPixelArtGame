using System;
using System.Numerics;

namespace _3DPixelArtEngine
{
    public class Ray
    {
        public Vector3 Origin;
        public Vector3 Direction;

        public Ray(Vector3 origin, Vector3 direction)
        {
            Origin = origin;
            Direction = direction;
        }

        public virtual void Rotate(Vector3 rotation)
        {
            rotation *= (float)Math.PI / 180f;

            Direction.Y = Direction.Y * (float)Math.Cos(rotation.X) - Direction.Z * (float)Math.Sin(rotation.X);
            Direction.Z = Direction.Y * (float)Math.Sin(rotation.X) + Direction.Z * (float)Math.Cos(rotation.X);

            Direction.X = Direction.X * (float)Math.Cos(rotation.Y) + Direction.Z * (float)Math.Sin(rotation.Y);
            Direction.Z = -Direction.X * (float)Math.Sin(rotation.Y) + Direction.Z * (float)Math.Cos(rotation.Y);

            Direction.X = Direction.X * (float)Math.Cos(rotation.Z) - Direction.Y * (float)Math.Sin(rotation.Z);
            Direction.Y = Direction.X * (float)Math.Sin(rotation.Z) + Direction.Y * (float)Math.Cos(rotation.Z);
        }

        public virtual void Translate(Vector3 translation)
        {
            Origin += translation;
        }

        public void MoveTo(Vector3 position, float distance = 0f)
        {
            Origin = position - Direction * distance;
        }

        public void RotateTo(Vector3 position)
        {
            Direction = Vector3.Normalize(position - Origin);
        }

        public Ray Clone()
        {
            return new Ray(Origin, Direction);
        }
    }
}
