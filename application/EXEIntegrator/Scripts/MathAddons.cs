using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXEIntegrator.Scripts
{
    static class MathAddons
    {
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }

        //
        public static float FloatListSum(this List<float> floats)
        {
            float temp = 0;
            for (int i = 0; i < floats.Count; i++)
            {
                temp += floats[i];
            }
            return temp;
        }
    }
}