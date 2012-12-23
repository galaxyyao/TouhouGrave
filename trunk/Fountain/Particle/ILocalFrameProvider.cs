using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.Particle
{
    public struct LocalFrame
    {
        public Vector4 Col0;
        public Vector4 Col1;
        public Vector4 Col2;
        public Vector4 Col3;
    }

    public interface ILocalFrameProvider
    {
        LocalFrame LocalFrame { get; }
    }
}
