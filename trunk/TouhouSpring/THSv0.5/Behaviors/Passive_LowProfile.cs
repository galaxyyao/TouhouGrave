using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public sealed class Passive_LowProfile : BaseBehavior<Passive_LowProfile.ModelType>,
        IGlobalEpilogTrigger<Commands.IMoveCard>,
        ILocalEpilogTrigger<Commands.IMoveCard>
    {
        private class LowProfileEffect : Undefendable, IStatusEffect
        {
            public string IconUri { get { return "atlas:Textures/Icons/Icons0$BTNInvisibility"; } }
            public string Text { get { return "超凡\n该角色无法被攻击。"; } }

            [BehaviorModel(typeof(LowProfileEffect), HideFromEditor = true)]
            new public class ModelType : Undefendable.ModelType { }
        }

        private LowProfileEffect m_effect;
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
            bool effective = Host.Owner.CardsOnBattlefield.Count == 1 && Host.Owner.CardsOnBattlefield[0] == Host;
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

        protected override void OnInitialize()
        {
            m_effect = new LowProfileEffect.ModelType().CreateInitialized() as LowProfileEffect;
            m_inEffect = false;
        }

        protected override void OnTransferFrom(IBehavior original)
        {
            var origBhv = original as Passive_LowProfile;
            m_inEffect = origBhv.m_inEffect;
            if (m_inEffect)
            {
                int undefendableIndex = origBhv.Host.Behaviors.IndexOf(origBhv.m_effect);
                Debug.Assert(undefendableIndex != -1 && Host.Behaviors[undefendableIndex] is LowProfileEffect);
                m_effect = Host.Behaviors[undefendableIndex] as LowProfileEffect;
            }
            else
            {
                m_effect = new LowProfileEffect.ModelType().CreateInitialized() as LowProfileEffect;
            }
       }

        [BehaviorModel(typeof(Passive_LowProfile), Category = "v0.5/Passive", DefaultName = "超凡")]
        public class ModelType : BehaviorModel
        { }
    }
}
