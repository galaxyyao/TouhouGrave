using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public static class LowProfile
    {
        public class Effect : BaseBehavior<Effect.ModelType>,
            IUndefendable, IStatusEffect
        {
            public string IconUri { get { return Model.IconUri; } }
            public string Text { get { return Model.Text; } }

            [BehaviorModel(typeof(Effect), HideFromEditor = true)]
            public class ModelType : BehaviorModel
            {
                public string IconUri { get; set; }
                public string Text { get; set; }
            }
        }

        public class ModelType : BehaviorModel
        {
            public string EffectIconUri
            {
                get { return EffectModel.IconUri; }
                set { EffectModel.IconUri = value; }
            }

            public string EffectIconText
            {
                get { return EffectModel.Text; }
                set { EffectModel.Text = value; }
            }

            [System.ComponentModel.Browsable(false)]
            public Effect.ModelType EffectModel
            {
                get; private set;
            }

            protected ModelType(string iconUri, string text)
            {
                EffectModel = new Effect.ModelType
                {
                    IconUri = iconUri,
                    Text = text
                };
            }
        }

        public class Behavior<T> : BaseBehavior<T>,
            IGlobalEpilogTrigger<Commands.IMoveCard>,
            ILocalEpilogTrigger<Commands.IMoveCard>
            where T : ModelType
        {
            private Effect m_effect;
            private bool m_inEffect;

            void IGlobalEpilogTrigger<Commands.IMoveCard>.RunGlobalEpilog(Commands.IMoveCard command)
            {
                if (command.Subject != null
                    && command.Subject.Owner == Host.Owner)
                {
                    // when some card of host's owner goes from/to battlefield
                    if ((command.FromZoneType == ZoneType.OnBattlefield) != (command.ToZoneType == ZoneType.OnBattlefield))
                    {
                        HaveEffect();
                    }
                }
            }

            void ILocalEpilogTrigger<Commands.IMoveCard>.RunLocalEpilog(Commands.IMoveCard command)
            {
                if (command.ToZoneType != ZoneType.OnBattlefield && m_inEffect)
                {
                    Game.QueueCommands(new Commands.RemoveBehavior(Host, m_effect));
                    m_inEffect = false;
                }
            }

            private void HaveEffect()
            {
                bool effective = IsEffective();
                if (effective != m_inEffect)
                {
                    if (effective)
                    {
                        Game.QueueCommands(new Commands.AddBehavior(Host, m_effect));
                    }
                    else
                    {
                        Game.QueueCommands(new Commands.RemoveBehavior(Host, m_effect));
                    }
                    m_inEffect = effective;
                }
            }

            protected virtual bool IsEffective() { return false; }

            protected override void OnInitialize()
            {
                m_effect = Model.EffectModel.CreateInitialized() as Effect;
                m_inEffect = false;
            }

            protected override void OnTransferFrom(IBehavior original)
            {
                var origBhv = original as Behavior<T>;
                m_inEffect = origBhv.m_inEffect;
                if (m_inEffect)
                {
                    int effectIndex = origBhv.Host.Behaviors.IndexOf(origBhv.m_effect);
                    Debug.Assert(effectIndex != -1 && Host.Behaviors[effectIndex] is Effect);
                    m_effect = Host.Behaviors[effectIndex] as Effect;
                }
                else
                {
                    m_effect = new Effect.ModelType().CreateInitialized() as Effect;
                }
            }
        }
    }
}
