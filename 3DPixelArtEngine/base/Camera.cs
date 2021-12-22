using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Camera
    {
        public Vector3 Point;
        public Ray[] Axes; // 0 - X, 1 - Y, 2 - Z


        public Camera(Vector3 point)
        {
            Point = point;
            Axes = new Ray[3];
            Axes[0] = new Ray(new Vector3(0f, 0f, 0f), new Vector3(1f, 0f, 0f));
            Axes[1] = new Ray(new Vector3(0f, 0f, 0f), new Vector3(0f, 1f, 0f));
            Axes[2] = new Ray(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f));
            // Camera is facing towards local X axis (the main ray), local Z is view window X and local Y is view window Y.
        }

        public void Rotate(Vector3 rotation) 
        {
            foreach (Ray r in Axes) 
            {
                r.Rotate(rotation);
            }
        }

        public void TranslateLocal(Vector3 translation)
        {
            Vector3 temp = new Vector3(0f, 0f, 0f);
            Ray[] ScaledAxes = new Ray[3];
            ScaledAxes[0] = new Ray(Axes[0].Point, new Vector3(Axes[0].Direction.X * translation.X, Axes[0].Direction.Y * translation.X, Axes[0].Direction.Z * translation.X));
            ScaledAxes[1] = new Ray(Axes[1].Point, new Vector3(Axes[1].Direction.X * translation.Y, Axes[1].Direction.Y * translation.Y, Axes[1].Direction.Z * translation.Y));
            ScaledAxes[2] = new Ray(Axes[2].Point, new Vector3(Axes[2].Direction.X * translation.Z, Axes[2].Direction.Y * translation.Z, Axes[2].Direction.Z * translation.Z));

            foreach (Ray r in ScaledAxes)
            {
                Point += r.Direction;
            }
        }

        public void TranslateGlobal(Vector3 translation)
        {
            Point += translation;
        }
    }
}
