using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Graphics
{
    class SwfInstance
    {
        private DDW.Display.DisplayObjectContainer m_container;
        private DDW.Display.Screen m_screen;

        public bool IsPlaying
        {
            get { return m_screen.isPlaying; }
            set { m_screen.PlayAll(); }
        }

        public float TimeFactor
        {
            get; set;
        }

        public SwfInstance(string assetName)
        {
            m_screen = new DDW.Display.Screen(new DDW.V2D.SymbolImport(assetName));
            m_container = new DDW.Display.DisplayObjectContainer();
            m_container.AddChild(m_screen);

            TimeFactor = 1.0f;
        }

        public void Update(float deltaTime)
        {
            var mspfBackup = DDW.Display.DisplayObject.MillisecondsPerFrame;
            DDW.Display.DisplayObject.MillisecondsPerFrame /= TimeFactor;
            m_screen.Update(deltaTime * 1000);
            DDW.Display.DisplayObject.MillisecondsPerFrame = mspfBackup;
        }

        public void Render(SpriteBatch spriteBatch)
        {
            m_screen.Draw(spriteBatch);
        }
    }
}
