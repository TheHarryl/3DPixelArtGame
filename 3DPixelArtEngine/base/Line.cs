using System.Numerics;

namespace _3DPixelArtEngine
{
    public class Line
    {
        public Vector3 Point1;
        public Vector3 Point2;

        public Line(Vector3 point1, Vector3 point2)
        {
            Point1 = point1;
            Point2 = point2;
        }
    }
}
