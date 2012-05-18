using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class RedFog : BaseBehavior<RedFog.ModelType>,
        ITrigger<Triggers.PlayerTurnStartedContext>,
        ITrigger<Triggers.PreCardDamageContext>,
        ITrigger<Triggers.PrePlayerDamageContext>,
        ITrigger<Triggers.PlayerTurnEndedContext>
    {
        private bool m_damageDealt = false;

        public void Trigger(Triggers.PlayerTurnStartedContext context)
        {
            if (IsOnBattlefield)
            {
                m_damageDealt = false;
            }
        }

        public void Trigger(Triggers.PreCardDamageContext context)
        {
            if (IsOnBattlefield
                && context.Cause is Warrior
                && context.Cause.Host.Owner == context.Game.PlayerPlayer)
            {
                m_damageDealt = true;
            }
        }

        public void Trigger(Triggers.PrePlayerDamageContext context)
        {
            if (IsOnBattlefield
                && context.Cause is Warrior
                && context.Cause.Host.Owner == context.Game.PlayerPlayer)
            {
                m_damageDealt = true;
            }
        }

        public void Trigger(Triggers.PlayerTurnEndedContext context)
        {
            if (IsOnBattlefield && !m_damageDealt)
            {
                context.Game.UpdateHealth(context.Game.PlayerPlayer, -Model.SelfDamage, this);
            }
        }

        [BehaviorModel("Red Fog", typeof(RedFog))]
        public class ModelType : BehaviorModel
        {
            public int SelfDamage { get; set; }
        }
    }
}
