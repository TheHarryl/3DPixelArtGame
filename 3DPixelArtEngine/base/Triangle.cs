using System;
using System.Numerics;

namespace _3DPixelArtEngine
{
    public class Triangle : Plane
    {
        public Triangle(Vector3 point1, Vector3 point2, Vector3 point3) : base(point1, point2, point3)
        {

        }

        public bool Contains(Triangle triangle)
        {
            return
                Contains(triangle.Point1, triangle.Point2) ||
                Contains(triangle.Point2, triangle.Point3) ||
                Contains(triangle.Point3, triangle.Point1) ||
                triangle.Contains(Point1, Point2) ||
                triangle.Contains(Point2, Point3) ||
                triangle.Contains(Point3, Point1);
        }

        public bool Contains(Vector3 point)
        {
            float Point1Weight = ((point.X * Point2.Y) - (point.Y * Point2.X) + ((((Point3.Y * Point2.X) - (Point3.X * Point2.Y)) * ((point.Z * Point2.Y) - (point.Y * Point2.Z) - (Point1.Z * Point2.Y))) / ((Point2.Y * Point3.Z) - (Point3.Y * Point2.Z)))) / (1 - ((((Point3.Y * Point2.X) - (Point3.X * Point2.Y)) * (Point1.Y * Point2.Z)) / ((Point2.Y * Point3.Z) - (Point3.Y * Point2.Z))));
            float Point3Weight = ((point.Z * Point2.Y) - (point.Y * Point2.Z) + (Point1Weight * ((Point1.Y * Point2.Z) - (Point1.Z * Point2.Y)))) / ((Point2.Y * Point3.Z) - (Point3.Y * Point2.Z));
            float Point2Weight = (point.Y - (Point1Weight * Point1.Y) - (Point3Weight * Point3.Y)) / Point2.Y;
            Console.WriteLine(Point1Weight + Point2Weight + Point3Weight);
            return Point1Weight >= 0f && Point2Weight >= 0f && Point3Weight >= 0f;
        }

        public bool Contains(Line line)
        {
            return Contains(new Vector(line.Point1, line.Point2));
        }

        public bool Contains(Vector3 point1, Vector3 point2)
        {
            return Contains(new Vector(point1, point2));
        }

        public bool Contains(Vector vector)
        {
            float det = -Vector3.Dot(vector.Direction, Normal);
            if (!(det >= 1e-6))
                return false;
            float invdet = 1f / det;
            Vector3 AO = vector.Origin - Point1;
            Vector3 DAO = Vector3.Cross(AO, vector.Direction);
            float u = Vector3.Dot(Point3 - Point1, DAO) * invdet;
            float v = -Vector3.Dot(Point2 - Point1, DAO) * invdet;
            float t = Vector3.Dot(AO, Normal) * invdet;
            return (t >= 0f && u >= 0f && v >= 0f && (u + v) <= 1f);
        }

        public bool Contains(Ray ray)
        {
            // https://stackoverflow.com/questions/42740765/intersection-between-line-and-triangle-in-3d

            float det = -Vector3.Dot(ray.Direction, Normal);
            if (!(det >= 1e-6))
                return false;
            float invdet = 1f / det;
            Vector3 AO = ray.Origin - Point1;
            Vector3 DAO = Vector3.Cross(AO, ray.Direction);
            float u = Vector3.Dot(Point3 - Point1, DAO) * invdet;
            float v = -Vector3.Dot(Point2 - Point1, DAO) * invdet;
            float t = Vector3.Dot(AO, Normal) * invdet;
            return (t >= 0f && u >= 0f && v >= 0f && (u + v) <= 1f);
        }
    }
}
