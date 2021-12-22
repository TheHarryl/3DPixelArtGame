using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace _3DPixelArtEngine
{
    public class Ray3 : Ray
    {
        public Ray LateralAxis;
        public Ray LongitudinalAxis;

        public Ray3(Vector3 origin) : base(origin, new Vector3(1f, 0f, 0f))
        {
            LateralAxis = new Ray(new Vector3(), new Vector3(0f, 0f, 1f));
            LongitudinalAxis = new Ray(new Vector3(), new Vector3(0f, 1f, 0f));
        }

        public override void Rotate(Vector3 rotation) 
        {
            base.Rotate(rotation);
            LateralAxis.Rotate(rotation);
            LongitudinalAxis.Rotate(rotation);
        }

        public override void Translate(Vector3 translation)
        {
            base.Translate(translation);
            LateralAxis.Translate(translation);
            LongitudinalAxis.Translate(translation);
        }

        public void TranslateLocal(Vector3 translation)
        {
            Origin += Direction * translation.X + LongitudinalAxis.Direction * translation.Y + LateralAxis.Direction * translation.Z;
        }
    }
}
