using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Mesh
    {
        public List<Triangle> Triangles;

        public Mesh(List<Triangle> triangles)
        {
            Triangles = triangles;
        }
    }
}
