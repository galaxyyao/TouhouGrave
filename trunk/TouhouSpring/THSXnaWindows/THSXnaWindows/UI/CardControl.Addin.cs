﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XnaMatrix = Microsoft.Xna.Framework.Matrix;

namespace TouhouSpring.UI
{
    partial class CardControl
    {
        public class Addin
        {
            public CardControl Control
            {
                get; private set;
            }

            public Services.CardDataManager.ICardData CardData
            {
                get { return Control.CardData; }
            }

            public Addin(CardControl control)
            {
                if (control == null)
                {
                    throw new ArgumentNullException("control");
                }

                Control = control;
            }

            public virtual void Update(float deltaTime) { }

            public virtual void RenderDepth(XnaMatrix transform, RenderEventArgs e) { }
            public virtual void RenderMain(XnaMatrix transform, RenderEventArgs e) { }
            public virtual void RenderPostMain(XnaMatrix transform, RenderEventArgs e) { }
        }
    }
}
