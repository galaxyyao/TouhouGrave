using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    partial class CardIcons
    {
        private class Icon : MouseTrackedControl
        {
            private Graphics.TexturedQuad m_icon;
            private Graphics.TextRenderer.IFormattedText m_counterNumber;

            public Behaviors.ICounter Counter;
            public Behaviors.IStatusEffect StatusEffect;
            public CardInstance Card;

            public Label TextLabel;
            public Panel TextPanel;

            public Color CounterNumberColor;
            public Graphics.TextRenderer.FormatOptions CounterNumberFormatOptions;
            public Graphics.TextRenderer.FormatOptions LabelTextFormatOptions;

            public void Initialize()
            {
                Region = new Rectangle(-IconSize / 2, 0, IconSize, IconSize);
                string iconUri = Counter != null ? Counter.IconUri : StatusEffect.IconUri;
                var texture = GameApp.Service<Services.ResourceManager>().Acquire<Graphics.VirtualTexture>(iconUri);
                m_icon = new Graphics.TexturedQuad(texture);
            }

            public void ReleaseTexture()
            {
                GameApp.Service<Services.ResourceManager>().Release(m_icon.Texture);
            }

            public override void OnMouseEnter(MouseEventArgs e)
            {
                string text = Counter != null ? Counter.Text : StatusEffect.Text;
                if (!String.IsNullOrEmpty(text))
                {
                    TextLabel.FormattedText = GameApp.Service<Graphics.TextRenderer>().FormatText(text, LabelTextFormatOptions);
                    var world2D = GameApp.Service<Services.GameUI>().InGameUIPage.Style.ChildIds["World2D"].Target;
                    var transform = TransformNode.GetTransformBetween(this, world2D);
                    var pt1 = transform.TransformCoord(new Vector3(0, 0, 0));
                    var pt2 = transform.TransformCoord(new Vector3(Region.Width, 0, 0));
                    var pt3 = transform.TransformCoord(new Vector3(0, Region.Height, 0));
                    var pt4 = transform.TransformCoord(new Vector3(Region.Width, Region.Height, 0));
                    float right = Math.Max(Math.Max(pt1.X, pt2.X), Math.Max(pt3.X, pt4.X));
                    float top = Math.Min(Math.Min(pt1.Y, pt2.Y), Math.Min(pt3.Y, pt4.Y));
                    TextPanel.Region = new Rectangle(0, 0,
                        TextLabel.FormattedText.Size.Width + LabelPanelPadding * 2,
                        TextLabel.FormattedText.Size.Height + LabelPanelPadding * 2);
                    TextPanel.Transform = MatrixHelper.Translate(right, top - TextPanel.Region.Height, 0);
                    TextLabel.Transform = MatrixHelper.Translate(right + LabelPanelPadding, top - TextPanel.Region.Height + LabelPanelPadding, 0);
                }
                else
                {
                    TextLabel.FormattedText = null;
                }
            }

            public override void OnMouseLeave(MouseEventArgs e)
            {
                TextLabel.FormattedText = null;
            }

            public void UpdateCounter()
            {
                if (Counter == null)
                {
                    return;
                }

                var number = Card.GetCounterCount(Counter.GetType()).ToString();
                if (number == "1")
                {
                    m_counterNumber = null;
                }
                else
                {
                    if (m_counterNumber == null || m_counterNumber.Text != number)
                    {
                        m_counterNumber = GameApp.Service<Graphics.TextRenderer>().FormatText(number, CounterNumberFormatOptions); 
                    }
                }

                var newWidth = IconSize + (m_counterNumber != null ? m_counterNumber.Size.Width + CounterNumberPadding : 0);
                Region = new Rectangle(-newWidth / 2, Region.Top, newWidth, Region.Height);
            }

            public void Draw()
            {
                var e = new RenderEventArgs();
                var transform = TransformToGlobal;
                e.RenderManager.Draw(new Graphics.TexturedQuad
                {
                    ColorScale = new Vector4(0, 0, 0, 1)
                }, Region, transform);
                e.RenderManager.Draw(m_icon, new Rectangle(Region.Right - IconSize, 0, IconSize, IconSize), transform);

                if (m_counterNumber != null)
                {
                    var drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                    drawOptions.ColorScaling = CounterNumberColor.ToVector4();
                    drawOptions.Offset = new Point(Region.Left + CounterNumberPadding, (Region.Top + Region.Bottom - m_counterNumber.Size.Height) / 2);
                    e.TextRenderer.DrawText(m_counterNumber, transform, drawOptions);
                }
            }
        }
    }
}
