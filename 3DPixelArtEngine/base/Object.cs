using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vector3 = System.Numerics.Vector3;

namespace _3DPixelArtEngine
{
    public class Object
    {
        private List<Triangle> _mesh;
        private List<Triangle> _transformedMesh;
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        public Texture2D TextureMap;
        public PointLight Light;

        public List<Triangle> Mesh
        {
            get => _transformedMesh;
            set
            {
                _mesh = value;
                TransformMesh();
            }
        }
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                TransformMesh();
            }
        }
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = new Vector3(value.X % 360, value.Y % 360, value.Z % 360);
                TransformMesh();
            }
        }
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                TransformMesh();
            }
        }

        public Object(Vector3 position = new Vector3(), Vector3 rotation = new Vector3())
        {
            _mesh = new List<Triangle>();
            _position = position;
            _rotation = rotation;
            _scale = new Vector3(1f, 1f, 1f);

            Light = new PointLight(Color.White, 0f, 0f);
        }

        private void TransformMesh()
        {
            _transformedMesh = new List<Triangle>();
            for (int i = 0; i < _mesh.Count; i++)
            {
                Vector vector1 = new Vector(Vector3.Zero, _mesh[i].Point1);
                Vector vector2 = new Vector(Vector3.Zero, _mesh[i].Point2);
                Vector vector3 = new Vector(Vector3.Zero, _mesh[i].Point3);
                vector1.Rotate(Rotation);
                vector2.Rotate(Rotation);
                vector3.Rotate(Rotation);
                _transformedMesh.Add(new Triangle(Position + vector1.Displacement * Scale.X, Position + vector2.Displacement * Scale.Y, Position + vector3.Displacement * Scale.Z));
            }
        }

        public List<Triangle> GetInvertedMesh()
        {
            List<Triangle> invertedTriangles = new List<Triangle>();
            for (int i = 0; i < Mesh.Count; i++)
            {
                invertedTriangles.Add(new Triangle(Mesh[i].Point1, Mesh[i].Point3, Mesh[i].Point2));
            }
            return invertedTriangles;
        }
    }
}
