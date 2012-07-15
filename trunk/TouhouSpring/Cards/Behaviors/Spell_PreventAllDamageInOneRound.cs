using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TouhouSpring.Behaviors
{
    public class Spell_PreventAllDamageInOneRound :
        BaseBehavior<Spell_PreventAllDamageInOneRound.ModelType>,
        ICastableSpell,
        ITrigger<Triggers.PrePlayerDamageContext>,
        ITrigger<Triggers.PreCardDamageContext>,
        ITrigger<Triggers.PlayerTurnEndedContext>
    {
        private bool m_isProtected = false;
        private Player m_currentPlayer;
        private Player m_spellCaster;

        public bool Cast(Game game, out string reason)
        {
            if (!game.PlayerPlayer.IsSkillCharged)
            {
                reason = "主角技能还没有被充能！";
                return false;
            }

            m_isProtected = true;
            m_currentPlayer = game.PlayerPlayer;
            m_spellCaster = Host.Owner;

            reason = String.Empty;
            return true;
        }

        public void Trigger(Triggers.PrePlayerDamageContext context)
        {
            if(m_isProtected&& context.PlayerToDamage==m_spellCaster)
                context.DamageToDeal = 0;
        }

        public void Trigger(Triggers.PreCardDamageContext context)
        {
            if (m_isProtected && context.CardToDamage.Owner == m_spellCaster)
                context.DamageToDeal = 0;
        }

        public void Trigger(Triggers.PlayerTurnEndedContext context)
        {
            if (context.Game.PlayerPlayer != m_currentPlayer && m_isProtected)
            {
                m_currentPlayer = null;
                m_isProtected = false;
                m_spellCaster = null;
            }
        }

        [BehaviorModel(typeof(Spell_PreventAllDamageInOneRound), DefaultName = "梦想封印")]
        public class ModelType : BehaviorModel
        { }
    }
}
