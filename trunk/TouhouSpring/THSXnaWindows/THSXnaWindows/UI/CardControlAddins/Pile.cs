﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GamePile = TouhouSpring.Pile;

namespace TouhouSpring.UI.CardControlAddins
{
    class Pile : CardControl.Addin, Style.IBindingProvider
    {
        private const int CardThickness = 10;

        private GamePile m_pile;
        private Graphics.VirtualTexture m_pileBackTexture;

        public Pile(CardControl control, GamePile pile)
            : base(control)
        {
            Control.Style.RegisterBinding(this);
            m_pile = pile;
        }

        public override void RenderPostMain(Matrix transform, RenderEventArgs e)
        {
            if (m_pileBackTexture == null)
            {
                return;
            }

            var pileSize = new Vector3(Control.Region.Width, Control.Region.Height, m_pile.Count * CardThickness);
            var transform1 = (Control.Style.ChildIds["Body"].Target as UI.ITransformNode).TransformToGlobal;
            GameApp.Service<Graphics.PileRenderer>().Draw(m_pileBackTexture, pileSize, Matrix.CreateTranslation(0, 0, -pileSize.Z) * transform1);
        }

        public override void Update(float deltaTime)
        {
            if (m_pileBackTexture == null)
            {
                var quad = (Control.Style.ChildIds["CardBack"].Target as UI.ComposedImage).Quads.First().TextureQuad;
                m_pileBackTexture = new Graphics.VirtualTexture(quad.Texture.XnaTexture,
                    new Microsoft.Xna.Framework.Rectangle(
                        (int)quad.UVBounds.Left + quad.Texture.Bounds.Left,
                        (int)quad.UVBounds.Top + quad.Texture.Bounds.Top,
                        (int)quad.UVBounds.Width,
                        (int)quad.UVBounds.Height));
            }
        }

        public bool TryGetValue(string id, out string replacement)
        {
            switch (id)
            {
                case "CardAnimator.Flip":
                case "CardAnimator.HighlightTransform":
                case "CardAnimator.LocationTransform":
                    replacement = Matrix.Identity.Serialize();
                    break;
                case "PileBackOffset":
                    replacement = (m_pile.Count * CardThickness).ToString();
                    break;
                default:
                    replacement = null;
                    return false;
            }

            return true;
        }
    }
}
