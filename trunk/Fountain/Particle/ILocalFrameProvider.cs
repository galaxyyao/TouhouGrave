using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle
{
    public struct LocalFrame
    {
        public Vector3 XAxis;
        public Vector3 YAxis;
    }

    public interface ILocalFrameProvider
    {
        LocalFrame LocalFrame { get; }
    }
}
