using System;
using System.Numerics;

namespace _3DPixelArtEngine
{
    public class PixelEngine
    {
        private int _width;
        private int _height;

        public Ray Camera;
        public float FieldOfView;

        public PixelEngine(int width, int height)
        {
            _width = width;
            _height = height;

            Camera = new Ray(new Vector3(), new Vector2());
            FieldOfView = 100f;

            
        }

        public Vector2 GamePixelToScreenPixel(Vector3 point)
        {
            return new Vector2();
        }
    }
}
