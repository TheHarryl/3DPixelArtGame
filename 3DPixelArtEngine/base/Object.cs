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
        public Mesh Mesh;
        public PointLight Light;

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
