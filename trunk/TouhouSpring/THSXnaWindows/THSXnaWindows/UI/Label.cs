using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.UI
{
    class Label : TransformNode, IRenderable
    {
        private Renderable m_renderable;

        public Graphics.TextRenderer.IFormatedText FormatedText
        {
            get; set;
        }

        public bool Shadowed
        {
            get; set;
        }

        public XnaColor TextColor
        {
            get; set;
        }

        public XnaColor ShadowTextColor
        {
            get; set;
        }

        public Point ShadowOffset
        {
            get; set;
        }

        public Label()
        {
            m_renderable = new Renderable(this);

            TextColor = XnaColor.Black;
        }

        public void OnRender(RenderEventArgs e)
        {
            if (FormatedText != null)
            {
                var transform = TransformToGlobal;
                Graphics.TextRenderer.DrawOptions drawOptions;
                if (Shadowed)
                {
                    drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                    drawOptions.ForcedColor = ShadowTextColor;
                    drawOptions.Offset = ShadowOffset;
                    GameApp.Service<Graphics.TextRenderer>().DrawText(FormatedText, transform, drawOptions);
                }
                drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = TextColor.ToVector4();
                GameApp.Service<Graphics.TextRenderer>().DrawText(FormatedText, transform, drawOptions);
            }
        }
    }
}
