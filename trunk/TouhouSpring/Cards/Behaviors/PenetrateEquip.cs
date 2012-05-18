using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class PenetrateEquip : BaseBehavior<PenetrateEquip.ModelType>,
        ITrigger<Triggers.PostCardPlayedContext>,
        ITrigger<Triggers.CardLeftBattlefieldContext>
    {
        private Penetrate m_penetrateEffect;

        public void Trigger(Triggers.PostCardPlayedContext context)
        {
            if (context.CardPlayed == Host)
            {
                m_penetrateEffect = new Penetrate();
                Host.Behaviors.Get<Equipment>().Holder.Behaviors.Add(m_penetrateEffect);
            }
        }

        public void Trigger(Triggers.CardLeftBattlefieldContext context)
        {
            if (context.Card == Host)
            {
                m_penetrateEffect.Host.Behaviors.Remove(m_penetrateEffect);
            }
        }

        [BehaviorModel("穿刺 (装备)", typeof(PenetrateEquip))]
        public class ModelType : BehaviorModel
        { }
    }
}
