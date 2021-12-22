using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Object
    {
        private List<Triangle> _triangles;
        public List<Triangle> Triangles
        {
            get
            {
                List<Triangle> transformedTriangles = new List<Triangle>(_triangles);
                for (int i = 0; i < transformedTriangles.Count; i++)
                {
                    /*Vector ray1 = new Vector(Orientation.Origin, transformedTriangles[i].Point1);
                    transformedTriangles[i].Point1 = Orientation.Origin + Vector3.Normalize*/
                }
                return transformedTriangles;
            }
            set => _triangles = value;
        }
        public Texture2D TextureMap;
        public Ray Orientation;

        public Object(List<Triangle> triangles)
        {
            _triangles = triangles;
        }

        public List<Triangle> GetInvertedMesh()
        {
            List<Triangle> invertedTriangles = new List<Triangle>();
            for (int i = 0; i < Triangles.Count; i++)
            {
                invertedTriangles.Add(new Triangle(Triangles[i].Point1, Triangles[i].Point3, Triangles[i].Point2));
            }
            return invertedTriangles;
        }
    }
}
