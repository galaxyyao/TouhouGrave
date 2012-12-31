using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class DamageIndicator : CardControl.Addin
    {
        [Services.LifetimeDependency(typeof(Services.ResourceManager))]
        private class Resources : Services.GameService
        {
            public Graphics.TextRenderer.FormatOptions FormatOptions
            {
                get; private set;
            }

            public Graphics.TextRenderer.DrawOptions DrawOptions
            {
                get; private set;
            }

            public Curve MoveCurve
            {
                get; private set;
            }

            public override void Startup()
            {
                var font = new Graphics.TextRenderer.FontDescriptor("Georgia", 20, System.Drawing.FontStyle.Bold);
                FormatOptions = new Graphics.TextRenderer.FormatOptions(font) { Alignment = Graphics.TextRenderer.Alignment.CenterMiddle };
                var drawOptions = Graphics.TextRenderer.DrawOptions.Default;
                drawOptions.ColorScaling = Vector4.One;
                DrawOptions = drawOptions;
                MoveCurve = GameApp.Service<Services.ResourceManager>().Acquire<Curve>("Curves/DamageIndicatorFloat");
            }
        }

        private int m_lastLife = -1;
        private Vector2 m_offset = new Vector2(0, -80);

        public DamageIndicator(CardControl control) : base(control)
        {
        }

        public override void Update(float deltaTime)
        {
            var warrior = Card.Behaviors.Get<Behaviors.Warrior>();
            if (warrior != null && m_lastLife > warrior.Life)
            {
                // get the center position of the card in screen space
                var pt = new Vector3(Control.Region.Width / 2, Control.Region.Height / 2, 0);
                pt = (Control.Style.ChildIds["Body"].Target as ITransformNode).TransformToGlobal.TransformCoord(pt);
                var halfVpWidth = GameApp.Instance.GraphicsDevice.Viewport.Width * 0.5f;
                var halfVpHeight = GameApp.Instance.GraphicsDevice.Viewport.Height * 0.5f;
                var screenPt = new Vector2(pt.X * halfVpWidth + halfVpWidth, halfVpHeight - pt.Y * halfVpHeight);

                var resources = GameApp.Service<Resources>();
                var text = GameApp.Service<Graphics.TextRenderer>().FormatText("[color:Red]-" + (m_lastLife - warrior.Life).ToString() + "[/color]", resources.FormatOptions);
                GameApp.Service<Graphics.FloatingText>().Register(text, resources.DrawOptions,
                    screenPt, screenPt + m_offset,
                    new Animation.CurveTrack(resources.MoveCurve), new Animation.ReverseLinearTrack(resources.MoveCurve.Keys.LastOrDefault().Position));
            }

            m_lastLife = warrior != null ? warrior.Life : -1;
        }
    }
}
