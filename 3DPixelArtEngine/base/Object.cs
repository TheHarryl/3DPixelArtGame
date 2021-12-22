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
        private List<Triangle> _transformedTriangles;
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        public List<Triangle> Triangles
        {
            get => _transformedTriangles;
            set => _triangles = value;
        }
        public Texture2D TextureMap;
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateTriangles();
            }
        }
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = new Vector3(value.X % 360, value.Y % 360, value.Z % 360);
                UpdateTriangles();
            }
        }
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateTriangles();
            }
        }

        public Object(List<Triangle> triangles, Vector3 position = new Vector3(), Vector3 rotation = new Vector3())
        {
            _triangles = triangles;
            Position = position;
            Rotation = rotation;
            Scale = new Vector3(1f, 1f, 1f);
        }

        private void UpdateTriangles()
        {
            _transformedTriangles = new List<Triangle>();
            for (int i = 0; i < _triangles.Count; i++)
            {
                Vector vector1 = new Vector(Vector3.Zero, _triangles[i].Point1);
                Vector vector2 = new Vector(Vector3.Zero, _triangles[i].Point2);
                Vector vector3 = new Vector(Vector3.Zero, _triangles[i].Point3);
                vector1.Rotate(Rotation);
                vector2.Rotate(Rotation);
                vector3.Rotate(Rotation);
                _transformedTriangles.Add(new Triangle(Position + vector1.Displacement * Scale.X, Position + vector2.Displacement * Scale.Y, Position + vector3.Displacement * Scale.Z));
            }
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
