using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Triangle
    {
        public Vector3 Point1;
        public Vector3 Point2;
        public Vector3 Point3;

        public Triangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            Point1 = point1;
            Point2 = point2;
            Point3 = point3;


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
            return false;
        }

        public bool Contains(Line line)
        {
            return Contains(line.Point1, line.Point2);
        }

        public bool Contains(Ray ray)
        {
            return false;
        }

        public bool Contains(Vector3 point1, Vector3 point2)
        {


            List<KeyValuePair<Vector3, Vector3>> sides = new List<KeyValuePair<Vector3, Vector3>>()
            {
                new KeyValuePair<Vector3, Vector3>(Point1, Point2),
                new KeyValuePair<Vector3, Vector3>(Point2, Point3),
                new KeyValuePair<Vector3, Vector3>(Point3, Point1)
            };

            int amount = 0;
            for (int i = 0; i < sides.Count; i++)
            {
                float min = Math.Min(sides[i].Key.Z, sides[i].Value.Z);
                float max = Math.Min(sides[i].Key.Z, sides[i].Value.Z);
                if (min <= point1.Z && max >= point1.Z)
                {
                    float slope = (sides[i].Key.X - sides[i].Value.X) / (sides[i].Key.Z - sides[i].Value.Z);
                    if (min + (slope * (point1.Z - min)) < point1.X)
                    {
                        amount++;
                    }
                }
            }
            return amount % 2 == 1;
        }

        private bool IntersectsPlane(Vector3 plane1, Vector3 plane2, Vector3 plane3, Vector3 line1, Vector3 line2)
        {
            Vector2 slopes = new Vector2((plane1 - plane2).Y / (plane1 - plane2).X, (plane1 - plane2).Z / (plane1 - plane2).X);
            
            return false;
        }
    }
}
