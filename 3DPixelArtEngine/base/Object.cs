using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Object
    {
        public List<Triangle> Triangles;
        public Texture2D TextureMap;
        public Ray Orientation;

        public Object(List<Triangle> triangles)
        {
            Triangles = triangles;
        }
    }
}
