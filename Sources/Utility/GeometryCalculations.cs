using Godot;
using System;
using System.Collections.Generic;

namespace Mars_Seal_Crimson
{
    public partial class GeometryCalculations
    {
        //Math.PI * degrees
        public static List<Godot.Vector2> GenerateCirclePlots(float radius, int numpoints)
        {
            List<Godot.Vector2> res = null;
            if ( (radius >= 0.001) && (numpoints >= 3))
            {
                res = new List<Vector2>();
                for (int ix = 0; ix < numpoints; ix++)
                {
                    float px = (float)Math.Cos(ix * Math.PI * 2 / numpoints);
                    float py = (float)Math.Sin(ix * Math.PI * 2 / numpoints);
                    res.Add(new Godot.Vector2(px, py) * radius);
                }
            }
            return res;
        }

        public static List<Godot.Vector3> Generate3DPlaneCirclePlots(float radius, int numpoints)
        {
            List<Godot.Vector3> res = null;
            if ((radius >= 0.001) && (numpoints >= 3))
            {
                res = new List<Vector3>();
                List<Godot.Vector2> zeroPlaneCircle = GenerateCirclePlots(radius, numpoints);
                if (zeroPlaneCircle != null)
                {

                }
            }
            return res;
        }
    }
}
