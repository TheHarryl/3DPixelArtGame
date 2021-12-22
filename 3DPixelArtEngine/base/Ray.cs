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

        /*public void RotateLateral(float deg)
        {
            float lateralCoefficient = Vector2.Distance(Vector2.Zero, new Vector2(Direction.X, Direction.Z));
            Vector2 lateralDirection = Vector2.Normalize(new Vector2(Direction.X, Direction.Z));
            float degrees = (float)Math.Acos(lateralDirection.X) + deg;
            Vector2 newDirection = new Vector2((float)Math.Cos(degrees), (float)Math.Sin(degrees));
            Direction.X = newDirection.X * lateralCoefficient;
            Direction.Z = newDirection.Y * lateralCoefficient;
        }*/

        public void Rotate(Vector3 rotation)
        {
            rotation *= (float)Math.PI / 180f;

            Direction.Y = Direction.Y * (float)Math.Cos(rotation.X) - Direction.Z * (float)Math.Sin(rotation.X);
            Direction.Z = Direction.Y * (float)Math.Sin(rotation.X) + Direction.Z * (float)Math.Cos(rotation.X);

            Direction.X = Direction.X * (float)Math.Cos(rotation.Y) + Direction.Z * (float)Math.Sin(rotation.Y);
            Direction.Z = -Direction.X * (float)Math.Sin(rotation.Y) + Direction.Z * (float)Math.Cos(rotation.Y);

            Direction.X = Direction.X * (float)Math.Cos(rotation.Z) - Direction.Y * (float)Math.Sin(rotation.Z);
            Direction.Y = Direction.Y * (float)Math.Sin(rotation.Z) + Direction.Y * (float)Math.Cos(rotation.Z);
        }

        public void MoveTo(Vector3 position, float distance = 0f)
        {
            Point = position - Direction * distance;
        }

        public void RotateTo(Vector3 position)
        {
            Direction = Vector3.Normalize(position - Point);
        }

        public Ray Clone()
        {
            return new Ray(Point, Direction);
        }
    }
}
