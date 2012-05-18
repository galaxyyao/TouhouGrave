using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
    public static class MathUtils
    {
        public const float PI = 3.14159265f;

        public static int RoundToNextPowerOfTwo(int n)
        {
            n--;
            n = (n >> 1) | n;
            n = (n >> 2) | n;
            n = (n >> 4) | n;
            n = (n >> 8) | n;
            n = (n >> 16) | n;
            n++;
            return n;
        }

        public static int AlignTo(int n, int boundary)
        {
            return (n + boundary - 1) & ~(boundary - 1);
        }
    }
}
