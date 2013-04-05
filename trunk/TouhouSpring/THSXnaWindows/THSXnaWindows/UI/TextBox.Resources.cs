using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.UI
{
    partial class TextBox
    {
        [Services.LifetimeDependency(typeof(Services.ResourceManager))]
        private class Resources : Services.GameService
        {
            public Graphics.VirtualTexture DashLine
            {
                get; private set;
            }

            public Graphics.VirtualTexture SolidLine
            {
                get; private set;
            }

            public override void Startup()
            {
                DashLine = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/UI/Common/DashLine");
                SolidLine = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>("Textures/UI/Common/SolidLine");
            }

            public override void Shutdown()
            {
                GameApp.Service<Services.ResourceManager>().Release(DashLine);
                GameApp.Service<Services.ResourceManager>().Release(SolidLine);
            }
        }
    }
}
