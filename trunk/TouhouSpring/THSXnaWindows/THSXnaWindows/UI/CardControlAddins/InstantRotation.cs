using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TouhouSpring.UI.CardControlAddins
{
    class InstantRotation : CardControl.Addin, Style.IBindingProvider
    {
        private Matrix m_transform;

        public bool Rotated
        {
            get; private set;
        }

        public InstantRotation(CardControl control) : base(control)
        {
            Control.Style.RegisterBinding(this);
            m_transform = Matrix.Identity;
        }

        public override void Update(float deltaTime)
        {
            var needRotate = !GameApp.Service<Services.GameUI>().ShallPlayerBeRevealed(Control.CardData.OwnerPlayerIndex);

            if (needRotate != Rotated)
            {
                Rotated = needRotate;
                SetTransform();
            }
        }

        public bool EvaluateBinding(string id, out string replacement)
        {
            switch (id)
            {
                case "CardAnimator.InstantRotation":
                    replacement = m_transform.Serialize();
                    return true;

                default:
                    replacement = null;
                    return false;
            }
        }

        private void SetTransform()
        {
            var halfWidth = Control.Region.Width * 0.5f;
            var halfHeight = Control.Region.Height * 0.5f;

            m_transform = Rotated
                          ? MatrixHelper.Translate(-halfWidth, -halfHeight)
                            * MatrixHelper.RotateZ(MathUtils.PI)
                            * MatrixHelper.Translate(halfWidth, halfHeight)
                          : MatrixHelper.Identity;
        }
    }
}
