using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Mesh
    {
        public Object Parent;
        private List<Triangle> _triangles;
        private List<Triangle> _transformedTriangles;
        private Vector3 _offset;
        private Vector3 _rotation;
        private Vector3 _scale;

        public Mesh(List<Triangle> triangles, Vector3 offset = new Vector3(), Vector3 rotation = new Vector3())
        {
            _offset = offset;
            _rotation = rotation;
            _scale = new Vector3(1f, 1f, 1f);
            SetTriangles(triangles);
        }

        public Mesh(string fileLocation, Vector3 offset = new Vector3(), Vector3 rotation = new Vector3())
        {
            _offset = offset;
            _rotation = rotation;
            _scale = new Vector3(1f, 1f, 1f);
            SetTriangles(fileLocation);
        }

        public void SetTriangles(List<Triangle> triangles)
        {
            _triangles = triangles;
            TransformMesh();
        }

        public void SetTriangles(string fileLocation)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> vertexNormals = new List<Vector3>();
            _triangles = new List<Triangle>();

            var lines = File.ReadLines(fileLocation);
            if (fileLocation.EndsWith(".obj"))
            {
                foreach (string line in lines)
                {
                    if (line.StartsWith("v "))
                    {
                        string[] args = line.Split(" ");
                        vertices.Add(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])));
                    }
                    if (line.StartsWith("vn"))
                    {
                        string[] args = line.Split(" ");
                        vertexNormals.Add(Vector3.Normalize(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3]))));
                    }
                    if (line.StartsWith("f "))
                    {
                        string[] argsStr = line.Split(" ");
                        string[][] args = new string[argsStr.Length][];
                        for (int i = 0; i < argsStr.Length; i++)
                        {
                            args[i] = argsStr[i].Split("/");
                        }

                        if (args.Length == 4) //three vertices => triangle
                        {
                            Triangle triangle = new Triangle(vertices[int.Parse(args[1][0]) - 1], vertices[int.Parse(args[2][0]) - 1], vertices[int.Parse(args[3][0]) - 1]);
                            if (args[1].Length == 3)
                            {
                                double angleBetweenNormals = Math.Acos(Vector3.Dot(Vector3.Normalize(vertexNormals[int.Parse(args[1][2]) - 1]), Vector3.Normalize(triangle.Normal)));
                                if (angleBetweenNormals > Math.PI / 2)
                                    triangle = new Triangle(vertices[int.Parse(args[2][0]) - 1], vertices[int.Parse(args[1][0]) - 1], vertices[int.Parse(args[3][0]) - 1]);
                            }
                            _triangles.Add(triangle);
                        }
                        else //otherwise, split into triangles using ears method
                        {
                            List<Vector3> polygonVertices = new List<Vector3>();
                            for (int i = 1; i < args.Length; i++)
                            {

                                polygonVertices.Add(vertices[int.Parse(args[i][0]) - 1]);
                            }
                            for (int i = 0; i < polygonVertices.Count; i++)
                            {
                                if (polygonVertices.Count == 3)
                                {
                                    Triangle triangle = new Triangle(polygonVertices[0], polygonVertices[1], polygonVertices[2]);
                                    if (args[1].Length == 3)
                                    {
                                        double angleBetweenNormals = Math.Acos(Vector3.Dot(Vector3.Normalize(vertexNormals[int.Parse(args[1][2]) - 1]), Vector3.Normalize(triangle.Normal)));
                                        if (angleBetweenNormals > Math.PI / 2)
                                            triangle = new Triangle(polygonVertices[1], polygonVertices[0], polygonVertices[2]);
                                    }
                                    _triangles.Add(triangle);

                                }
                                Triangle testTriangle;
                                if (i == 0)
                                {
                                    testTriangle = new Triangle(polygonVertices[polygonVertices.Count - 1], polygonVertices[i], polygonVertices[i + 1]);
                                }
                                else if (i == polygonVertices.Count - 1)
                                {
                                    testTriangle = new Triangle(polygonVertices[i - 1], polygonVertices[i], polygonVertices[0]);
                                }
                                else
                                {
                                    testTriangle = new Triangle(polygonVertices[i - 1], polygonVertices[i], polygonVertices[i + 1]);
                                }
                                bool isEar = true;
                                for (int j = 0; j < polygonVertices.Count; j++)
                                {
                                    if (j == i)
                                        continue;
                                    if (testTriangle.Contains(polygonVertices[j]))
                                        isEar = false;
                                }
                                if (isEar)
                                {
                                    if (args[1].Length == 3)
                                    {
                                        double angleBetweenNormals = Math.Acos(Vector3.Dot(Vector3.Normalize(vertexNormals[int.Parse(args[1][2]) - 1]), Vector3.Normalize(testTriangle.Normal)));
                                        if (angleBetweenNormals > Math.PI / 2)
                                            testTriangle = new Triangle(testTriangle.Point2, testTriangle.Point1, testTriangle.Point3);
                                    }
                                    _triangles.Add(testTriangle);
                                    polygonVertices.RemoveAt(i);
                                    i = 0;
                                }
                            }
                        }
                    }
                }
            }
            TransformMesh();
        }

        public List<Triangle> GetTriangles()
        {
            return _transformedTriangles;
        }

        public List<Triangle> GetInvertedTriangles()
        {
            List<Triangle> invertedTriangles = new List<Triangle>();
            for (int i = 0; i < _triangles.Count; i++)
            {
                invertedTriangles.Add(new Triangle(_triangles[i].Point1, _triangles[i].Point3, _triangles[i].Point2));
            }
            return invertedTriangles;
        }

        protected void TransformMesh()
        {
            Vector3 position = Offset + (this.Parent == null ? Vector3.Zero : this.Parent.Position);
            Vector3 rotation = Rotation + (this.Parent == null ? Vector3.Zero : this.Parent.Rotation);
            _transformedTriangles = new List<Triangle>();
            for (int i = 0; i < _triangles.Count; i++)
            {
                Vector vector1 = new Vector(Vector3.Zero, _triangles[i].Point1);
                Vector vector2 = new Vector(Vector3.Zero, _triangles[i].Point2);
                Vector vector3 = new Vector(Vector3.Zero, _triangles[i].Point3);
                vector1.Rotate(rotation);
                vector2.Rotate(rotation);
                vector3.Rotate(rotation);
                _transformedTriangles.Add(new Triangle(position + vector1.Displacement * Scale.X, position + vector2.Displacement * Scale.Y, position + vector3.Displacement * Scale.Z));
            }
        }

        public Vector3 Offset
        {
            get => _offset;
            set
            {
                _offset = value;
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
    }
}
