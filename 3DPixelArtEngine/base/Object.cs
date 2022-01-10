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
        private Mesh _mesh;
        private PointLight _light;

        public Mesh Mesh {
            get => _mesh;
            set {
                _mesh = value;
                _mesh.Parent = this;
            }
        }
        public PointLight Light
        {
            get => _light;
            set
            {
                _light = value;
                _light.Parent = this;
            }
        }

        private Vector3 _position;
        private Vector3 _rotation;

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                if (Mesh != null)
                    Mesh.Offset = Mesh.Offset;
            }
        }
        public Vector3 Rotation
        {
            get => _rotation;
            set
            {
                _rotation = new Vector3(value.X % 360, value.Y % 360, value.Z % 360);
                if (Mesh != null)
                    Mesh.Rotation = Mesh.Rotation;
            }
        }

        public Object()
        {
            
        }

        public bool CollidesWith(Object target)
        {
            if (target.Mesh == null)
                return false;
            return CollidesWith(target.Mesh);
        }

        public bool CollidesWith(Mesh mesh)
        {
            if (Mesh == null)
                return false;
            for (int i = 0; i < mesh.GetTriangles().Count; i++)
            {
                for (int v = 0; v < Mesh.GetTriangles().Count; v++)
                {
                    if (mesh.GetTriangles()[i].Contains(Mesh.GetTriangles()[v]))
                        return true;
                }
            }
            return false;
        }
    }
}
