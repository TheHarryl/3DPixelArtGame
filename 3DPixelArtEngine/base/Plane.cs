using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Plane
    {
        protected Vector3 _point1;
        protected Vector3 _point2;
        protected Vector3 _point3;

        public Vector3 Point1
        {
            get => _point1;
            set
            {
                _point1 = value;
                Center = (_point1 + _point2 + _point3) / 3f;
                Normal = Vector3.Cross(Point2 - Point1, Point3 - Point1);
                
            }
        }
        public Vector3 Point2
        {
            get => _point2;
            set
            {
                _point2 = value;
                Center = (_point1 + _point2 + _point3) / 3f;
                Normal = Vector3.Cross(Point2 - Point1, Point3 - Point1);
            }
        }
        public Vector3 Point3
        {
            get => _point3;
            set
            {
                _point3 = value;
                Center = (_point1 + _point2 + _point3) / 3f;
                Normal = Vector3.Cross(Point2 - Point1, Point3 - Point1);
            }
        }

        public Vector3 Center;
        public Vector3 Normal;

        public Plane(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            _point1 = point1;
            _point2 = point2;
            _point3 = point3;

            Center = (point1 + point2 + point3) / 3f;
            Normal = Vector3.Cross(Point2 - Point1, Point3 - Point1);
        }

        public Vector3 GetIntersection(Ray ray)
        {
            // https://stackoverflow.com/questions/42740765/intersection-between-line-and-triangle-in-3d

            Vector3 q1 = ray.Origin - ray.Direction * 100000f;
            Vector3 q2 = ray.Origin + ray.Direction * 100000f;
            float t = -Vector3.Dot(q1 - Point1, Normal) / Vector3.Dot(q2 - q1, Normal);
            return q1 + t * (q2 - q1);
        }

        public Ray GetReflection(Ray ray)
        {
            Vector3 intersection = this.GetIntersection(ray);
            Vector3 direction = ray.Direction - (2f * Vector3.Dot(ray.Direction, Normal) * Normal);
            return new Ray(intersection, direction);
        }
    }
}
